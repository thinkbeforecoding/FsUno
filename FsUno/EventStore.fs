namespace FsUno

open System

exception WrongExpectedVersion

// Warning:
// This is a *TOY* implementation of an in memory version of an eventstore
// The presence here is just to be able to run the sample without
// runing an external event store.
// 
// For a production ready implementation, see the EventStore module below
module ToyInMemoryEventStore =
    type Stream = { mutable Events:  (Event * int) list }
        with
        static member version stream = 
            stream.Events
            |> Seq.last
            |> snd
    

    type InMemoryEventStore = 
        { mutable streams : Map<string,Stream> 
          projection : Event -> unit }

        interface IDisposable
            with member x.Dispose() = ()         

    let create() = { streams = Map.empty
                     projection = fun _ -> () }
    let subscribe projection store =
        { store with projection = projection} 

    let readStream store streamId version count =
        match store.streams.TryFind streamId with
        | Some(stream) -> 
            let events =
                stream.Events
                |> Seq.skipWhile (fun (_,v) -> v < version )
                |> Seq.takeWhile (fun (_,v) -> v <= version + count)
                |> Seq.toList 
            let lastEventNumber = events |> Seq.last |> snd 
            
            events |> List.map fst,
                lastEventNumber ,
                if lastEventNumber < version + count 
                then None 
                else Some (lastEventNumber+1)
            
        | None -> [], -1, None

    let appendToStream store streamId expectedVersion newEvents =
        let eventsWithVersion =
            newEvents
            |> List.mapi (fun index event -> (event, expectedVersion + index + 1))

        match store.streams.TryFind streamId with
        | Some stream when Stream.version stream = expectedVersion -> 
            stream.Events <- stream.Events @ eventsWithVersion
        
        | None when expectedVersion = -1 -> 
            store.streams <- store.streams.Add(streamId, { Events = eventsWithVersion })        

        | _ -> raise WrongExpectedVersion 
        
        newEvents
        |> Seq.iter store.projection





// This module uses a production ready event store
// Your constraint may vary, but this version should
// be good enough to start.
module EventStore =
    open System
    open System.Net
    open EventStore.ClientAPI
    open Serialization
    let deserialize (event: ResolvedEvent) = deserializeUnion event.Event.EventType event.Event.Data
    let serialize event = 
        let typeName, data = serializeUnion event
        EventData(Guid.NewGuid(), typeName, true, data, null)

    let create() = 
        let s = EventStoreConnection.Create(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113))
        s.Connect()
        s

    let subscribe (projection: Event -> unit) (store: IEventStoreConnection) =


        let credential = SystemData.UserCredentials("admin", "changeit")
        store.SubscribeToAll(true, (fun s e -> deserialize e |> Option.iter projection), userCredentials = credential) |> ignore
        store

    let readStream (store: IEventStoreConnection) streamId version count =
        let slice = store.ReadStreamEventsForward(streamId, version, 500, true)

        let events = 
            slice.Events 
            |> Seq.choose deserialize
            |> Seq.toList
        
        let nextEventNumber = 
            if slice.IsEndOfStream 
            then None 
            else Some slice.NextEventNumber

        events, slice.LastEventNumber, nextEventNumber   

    let appendToStream (store: IEventStoreConnection) streamId expectedVersion newEvents =
        let serializedEvents = Seq.map serialize newEvents
        store.AppendToStream(streamId, expectedVersion, serializedEvents)

// This module implements AwaitTask for non generic Task
// It should be useless in F# 4 since it should be implemented in FSharp.Core
[<AutoOpen>]
module AsyncExtensions =
    open System
    open System.Threading
    open System.Threading.Tasks
    type Microsoft.FSharp.Control.Async with
        static member Raise(ex) = Async.FromContinuations(fun (_,econt,_) -> econt ex)

        static member AwaitTask (t: Task) =
            let tcs = new TaskCompletionSource<unit>(TaskContinuationOptions.None)
            t.ContinueWith((fun _ -> 
                if t.IsFaulted then tcs.SetException t.Exception
                elif t.IsCanceled then tcs.SetCanceled()
                else tcs.SetResult(())), TaskContinuationOptions.ExecuteSynchronously) |> ignore
            async {
                try
                    do! Async.AwaitTask tcs.Task
                with
                | :? AggregateException as ex -> 
                    do! Async.Raise (ex.Flatten().InnerExceptions |> Seq.head) }


// This module uses a production ready event store
// This version use the async API
module Async = 
    module EventStore =
        open System
        open System.Net
        open EventStore.ClientAPI
        open Serialization

        type IEventStoreConnection with
            member this.AsyncConnect() = Async.AwaitTask(this.ConnectAsync())
            member this.AsyncReadStreamEventsForward stream start count resolveLinkTos =
                Async.AwaitTask(this.ReadStreamEventsForwardAsync(stream, start, count, resolveLinkTos))
            member this.AsyncAppendToStream stream expectedVersion events =
                Async.AwaitTask(this.AppendToStreamAsync(stream, expectedVersion, events))

        let create = EventStore.create 

        let subscribe = EventStore.subscribe
        
        let readStream (store: IEventStoreConnection) streamId version count = 
            async {
                let! slice = store.AsyncReadStreamEventsForward streamId version 500 true

                let events = 
                    slice.Events 
                    |> Seq.choose EventStore.deserialize
                    |> Seq.toList
                
                let nextEventNumber = 
                    if slice.IsEndOfStream 
                    then None 
                    else Some slice.NextEventNumber

                return events, slice.LastEventNumber, nextEventNumber }

        let appendToStream (store: IEventStoreConnection) streamId expectedVersion newEvents = 
            async {
                let serializedEvents = [| for event in newEvents -> EventStore.serialize event |]

                do! store.AsyncAppendToStream streamId expectedVersion serializedEvents }




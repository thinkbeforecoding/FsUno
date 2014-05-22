namespace FsUno

open System

exception WrongExpectedVersion


module InMemoryEventStore =
    type Stream = { mutable Events:  (Event * int) list }
        with
        static member version stream = 
            stream.Events
            |> Seq.last
            |> snd
    

    type InMemoryEventStore = 
        { mutable streams : Map<string,Stream> }
        interface IDisposable
            with member x.Dispose() = ()         

    let create() = { streams = Map.empty }

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














module EventStore =
    open System
    open System.Net
    open System.IO
    open System.Reflection
    open EventStore.ClientAPI
    open Newtonsoft.Json

    let create() = 
        let s = EventStoreConnection.Create(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113))
        s.Connect()
        s

    let serializer = JsonSerializer.Create()

    let deserialize (event: ResolvedEvent) =
        let event = event.Event
        let t = Assembly.GetExecutingAssembly().GetType("Events+Event+" + event.EventType) 
        if t = null then
            None
        else
            use stream = new MemoryStream(event.Data);
            use reader = new StreamReader(stream)
            Some (serializer.Deserialize(reader, t) :?> Event)

    let serialize (event: Event) =
        use stream = new MemoryStream()
        use writer = new StreamWriter(stream)
        serializer.Serialize(writer, event)
        writer.Flush()
        EventData(
            Guid.NewGuid(),
            event.GetType().Name,
            true,
            stream.ToArray(),
            null )

 
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

        newEvents
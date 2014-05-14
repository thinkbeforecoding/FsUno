namespace FsUno

exception WrongExpectedVersion

type IEventStore =
    abstract member FoldEvents : fold: ('T -> Event -> 'T) -> seed: 'T -> streamId: string -> version: int -> int * 'T
    abstract member SaveEvents : streamId: string -> expectedVersion: int -> events: Event list -> unit

type IPublisher =
    abstract member Publish : event: Event -> unit
















type Stream = { mutable Events:  (Event * int) list }

type InMemoryEventStore(publisher: IPublisher) =
    let mutable streams = Map.empty

    interface IEventStore with
        member this.FoldEvents fold seed streamId version =
            match streams.TryFind streamId with
            | Some(stream) -> 
                seq { 
                    for event, eventVersion in stream.Events do
                    if eventVersion >= version then
                        yield eventVersion, event }
                |> Seq.toList 
                |> List.rev
                |> Seq.fold (fun (_,s) (v,e) -> v, fold s e) (-1, seed)
            | None -> -1, seed

        member this.SaveEvents streamId expectedVersion newEvents =
            let eventsWithVersion =
                newEvents
                |> List.mapi (fun index event -> (event, expectedVersion + index + 1))

            match streams.TryFind streamId with
            | Some({Events = (event, eventVersion) :: _} as stream) 
              when eventVersion = expectedVersion-> 
                stream.Events <- eventsWithVersion @ stream.Events
            
            | None when expectedVersion = -1 -> 
                streams <- streams.Add(streamId, { Events = eventsWithVersion })        

            | _ -> raise WrongExpectedVersion 
            
            newEvents
            |> List.iter publisher.Publish



















open System
open System.Net
open System.IO
open System.Reflection
open EventStore.ClientAPI
open Newtonsoft.Json

type EventStore (publisher: IPublisher) =
    let store = 
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
            Some (event.EventNumber, serializer.Deserialize(reader, t) :?> Event)

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

    interface IDisposable with
        member this.Dispose() = store.Close()
 
    interface IEventStore with
        member this.FoldEvents fold seed streamId version =
            let rec getEvents position = 
                seq {
                    let slice = store.ReadStreamEventsForward(streamId, position, 500, true)
                    yield! slice.Events
                    if not slice.IsEndOfStream then
                        yield! getEvents slice.NextEventNumber
                }
            getEvents version
            |> Seq.choose deserialize
            |> Seq.fold (fun (_,s) (v,e) -> v, fold s e) (0,seed)


        member this.SaveEvents streamId expectedVersion newEvents =
            let serializedEvents = Seq.map serialize newEvents

            store.AppendToStream(streamId, expectedVersion, serializedEvents)

            List.iter publisher.Publish newEvents
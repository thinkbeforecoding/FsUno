namespace FsUno

exception WrongExpectedVersion

type IEventStore =
    abstract member GetEvents : streamId: string -> version: int -> Event option list
    abstract member SaveEvents : streamId: string -> expectedVersion: int -> events: Event list -> unit

type IPublisher =
    abstract member Publish : event: Event -> unit
















type Stream =
    {
        mutable Events:  (Event * int) list
    }

type InMemoryEventStore(publisher: IPublisher) =
    let mutable streams = Map.empty

    interface IEventStore with
        member this.GetEvents streamId version =
            match streams.TryFind streamId with
            | Some(stream) -> 
                seq { 
                    for event, eventVersion in stream.Events do
                    if eventVersion >= version then
                        yield Some event }
                |> Seq.toList 
                |> List.rev
            
            | None -> []

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
open ServiceStack.Text

[<CLIMutable>]
type CardData =
    {
        Type: string
        Digit: Nullable<int>
        Color: string
    }


type EventStore (publisher: IPublisher) =
    let store = EventStoreConnection.Create()

    let deserialize (event: ResolvedEvent) =
        let event = event.Event
        let t = Assembly.GetExecutingAssembly().GetType(event.EventType) 
        if t = null then
            None
        else
            use stream = new MemoryStream(event.Data);
            Some (JsonSerializer.DeserializeFromStream(t, stream) :?> Event)

    let serialize (event: Event) =
        use stream = new MemoryStream()
        JsonSerializer.SerializeToStream(event, stream)

        EventData(
            Guid.NewGuid(),
            event.GetType().FullName,
            true,
            stream.ToArray(),
            null
        )

    
    member this.Start() =
        JsConfig<Card>.RawSerializeFn <- fun (c: Card) ->
            match c with
            | Digit(n, color) -> JsonSerializer.SerializeToString({Type = "Digit"; Digit= Nullable n; Color= sprintf "%A" color})
            | KickBack(color) -> JsonSerializer.SerializeToString({ Type = "KickBack"; Color = sprintf "%A" color; Digit = Nullable<int>()})
            | _ -> "{}"
       
        let a = fun text -> 
                let d = JsonSerializer.DeserializeFromString<CardData>(text)
                let color = 
                    match d.Color with
                    | "Red" -> Red
                    | "Green" -> Green
                    | "Blue" -> Blue
                    | "Yellow" -> Yellow
                    | _ -> raise (Exception("Unknown color"))
                match d.Type with
                | "Digit" -> Digit(d.Digit.Value, color)
                | "KickBack" -> KickBack(color)
                | _ -> raise (Exception("Unknown card type"))
                 
        JsConfig<Card>.RawDeserializeFn <- new Func<string,Card>(a)
            
        store.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113))
        

    member this.Stop() =
        store.Close()
 
    interface IEventStore with
        member this.GetEvents streamId version =
            let rec getEvents position = 
                seq {
                    let slice = store.ReadStreamEventsForward(streamId, position, 500, true)
                    yield! slice.Events
                    if not slice.IsEndOfStream then
                        yield! getEvents slice.NextEventNumber
                }
            getEvents version
            |> Seq.map deserialize
            |> Seq.toList

        member this.SaveEvents streamId expectedVersion newEvents =
            let serializedEvents =
                newEvents
                |> Seq.map serialize

            if expectedVersion = 0 then
                store.CreateStream(streamId, Guid.NewGuid(), true, [||])

            store.AppendToStream(streamId, expectedVersion, serializedEvents)

            newEvents
            |> List.iter publisher.Publish

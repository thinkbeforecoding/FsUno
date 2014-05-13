namespace FsUno

module Option =
    let apply f state =
        function
        | Some v -> f state v
        | None -> state

module List =
    let foldi f state = 
        List.fold 
            (fun (version, state) value -> version+1, Option.apply f state value ) 
            (-1, state)


type DiscardPileRepository(store: IEventStore) =

    let streamId gameId = sprintf "DiscardPile%d" gameId 

    member this.GetById gameId =
        store.GetEvents (streamId gameId) 0
        |> List.foldi apply empty
        

    member this.Save gameId expectedVersion events =
        store.SaveEvents (streamId gameId) expectedVersion events
        
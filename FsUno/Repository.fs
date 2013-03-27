namespace FsUno

type DiscardPileRepository(store: IEventStore) =

    let streamId gameId = sprintf "DiscardPile%d" gameId 

    member this.GetById gameId =
        store.GetEvents (streamId gameId) 0
        |> List.fold (fun (version, state) event -> (version+1, apply state event)) (-1, empty)
        

    member this.Save gameId expectedVersion events =
        store.SaveEvents (streamId gameId) expectedVersion events
        





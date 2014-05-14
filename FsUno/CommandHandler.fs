module FsUno.DiscardPileCommandHandler

open DiscardPile

let create (store: IEventStore) =

    // this is the "repository"
    let streamId gameId = sprintf "DiscardPile%d" gameId 
    let getById gameId = store.FoldEvents apply empty (streamId gameId) 0
    let save gameId expectedVersion events = store.SaveEvents (streamId gameId) expectedVersion events

    let handle' c v s = v, handle c s

    fun command ->
        let id = gameId command

        getById id
        ||> handle' command
        ||> save id
        
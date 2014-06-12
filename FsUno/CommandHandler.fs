module FsUno.DiscardPileCommandHandler

open DiscardPile

let create readStream appendToStream =

    // this is the "repository"
    let streamId gameId = sprintf "DiscardPile-%d" gameId 
    let load gameId =
        let rec fold state version =
            let events, lastEvent, nextEvent = readStream (streamId gameId) version 500
            let state = List.fold apply state events
            match nextEvent with
            | None -> lastEvent, state
            | Some n -> fold state n
        fold empty 0

    let save gameId expectedVersion events = appendToStream (streamId gameId) expectedVersion events

    // the /|> operator works on a pair. It applies the function on the second element.
    let inline (/|>) (v,s) f = v, f s    

    fun command ->
        let id = gameId command

        load id
        /|> handle command
        ||> save id
        
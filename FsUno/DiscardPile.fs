[<AutoOpen>]
module DiscardPile

type State = {
    GameId: GameId
    GameAlreadyStarted: bool
    PlayerCount: int
    CurrentPlayer: int
    TopCard: Card }

let empty = {
    State.GameId = 0
    GameAlreadyStarted = false
    PlayerCount = 0
    CurrentPlayer = 0
    TopCard = Digit(0,Red) }

// Operations on the DiscardPile aggregate

let startGame gameId playerCount firstCard state =
    if playerCount <= 2 then invalidArg "playerCount" "You should be at least 3 players"
    if state.GameAlreadyStarted then invalidOp "You cannot start game twice"

    [ GameStarted(gameId, playerCount, firstCard) ]


















let playCard gameId player card state =
    if player <> state.CurrentPlayer then invalidOp "Player should play at his turn"

    match card, state.TopCard with
    | Digit(n1, color1), Digit(n2, color2) when n1 = n2 || color1 = color2 ->
        [ CardPlayed(gameId, player, card )]
    | _ -> invalidOp "Play same color or same value !"















// Applies state changes for events

let apply state event =
    match event with
    | GameStarted(id, playerCount, firstCard) -> 
        { state with 
            GameId = id
            GameAlreadyStarted = true
            PlayerCount = playerCount
            CurrentPlayer = 0
            TopCard = firstCard }
    | CardPlayed(id, player, card) ->
        { state with
            CurrentPlayer = 
                (state.CurrentPlayer + 1) % state.PlayerCount
            TopCard = card }

// Replays all events from start to get current state

let replay events = List.fold apply empty events

// Map commands to aggregates operations

let handle command =
    match command with
    | StartGame(id, playerCount, firstCard) -> startGame id playerCount firstCard
    | PlayCard(id, player, card) -> playCard id player card


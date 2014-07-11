[<AutoOpen>]
module DiscardPile

// A type representing current player turn
// All operation should be done inside the module

type Turn = (*player*)int * (*playerCount*)int

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Turn =
    let empty = (0,1)
    let start count = (0, count)
    let next (player, count) = (player + 1) % count, count
    let isNot p (current, _) = p <> current













    

type State = {
    GameAlreadyStarted: bool
    Player: Turn
    TopCard: Card }

let empty = {
    GameAlreadyStarted = false
    Player = Turn.empty
    TopCard = Digit(0,Red) }

// Operations on the DiscardPile aggregate

let startGame gameId playerCount firstCard state =
    if playerCount <= 2 then invalidArg "playerCount" "You should be at least 3 players"
    if state.GameAlreadyStarted then invalidOp "You cannot start game twice"

    [ GameStarted(gameId, playerCount, firstCard) ]


















let playCard gameId player card state =
    if state.Player |> Turn.isNot player then invalidOp "Player should play at his turn"

    match card, state.TopCard with
    | Digit(n1, color1), Digit(n2, color2) when n1 = n2 || color1 = color2 ->
        [ CardPlayed(gameId, player, card )]
    | _ -> invalidOp "Play same color or same value !"















// Map commands to aggregates operations

let handle =
    function
    | StartGame(id, playerCount, firstCard) -> startGame id playerCount firstCard
    | PlayCard(id, player, card) -> playCard id player card














// Applies state changes for events

let apply state =
    function
    | GameStarted(_, playerCount, firstCard) -> 
        { GameAlreadyStarted = true
          Player = Turn.start playerCount
          TopCard = firstCard }

    | CardPlayed(_, _, card) ->
        { state with
            Player = state.Player |> Turn.next 
            TopCard = card }

// Replays all events from start to get current state

let replay events = List.fold apply empty events



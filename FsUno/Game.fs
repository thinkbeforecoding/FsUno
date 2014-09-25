module Game

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
    with
    static member initial = {
        GameAlreadyStarted = false
        Player = Turn.empty
        TopCard = Digit(0,Red) }

// Operations on the DiscardPile aggregate

let startGame (command: StartGame) state =
    if command.PlayerCount <= 2 then invalidArg "playerCount" "You should be at least 3 players"
    if state.GameAlreadyStarted then invalidOp "You cannot start game twice"

    [ GameStarted { GameId = command.GameId
                    PlayerCount = command.PlayerCount
                    FirstCard = command.FirstCard } ]


















let playCard (command: PlayCard) state =
    if state.Player |> Turn.isNot command.Player then invalidOp "Player should play at his turn"

    match command.Card, state.TopCard with
    | Digit(n1, color1), Digit(n2, color2) when n1 = n2 || color1 = color2 ->
        [ CardPlayed { GameId = command.GameId
                       Player = command.Player
                       Card = command.Card }]
    | _ -> invalidOp "Play same color or same value !"















// Map commands to aggregates operations

let handle =
    function
    | StartGame command -> startGame command 
    | PlayCard command -> playCard command 














// Applies state changes for events

let evolve state =
    function
    | GameStarted e -> 
        { GameAlreadyStarted = true
          Player = Turn.start e.PlayerCount
          TopCard = e.FirstCard }

    | CardPlayed e ->
        { state with
            Player = state.Player |> Turn.next 
            TopCard = e.Card }




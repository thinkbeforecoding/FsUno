[<AutoOpen>]
module DiscardPile

type State =
    {
        GameId: GameId
        GameAlreadyStarted: bool
        PlayerCount: int
        CurrentPlayer: int
        TopCard: Card
        Direction: Direction
    }

let empty =
    {
        State.GameId = 0
        GameAlreadyStarted = false
        PlayerCount = 0
        CurrentPlayer = 0
        TopCard = Digit(0,Red)
        Direction = ClockWise
    }


// Operations on the DiscardPile aggregate

let startGame gameId playerCount firstCard state : Event list =
    if playerCount <= 2 then invalidArg "playerCount" "You should be at least 3 players"
    if state.GameAlreadyStarted then invalidOp "You cannot start game twice"

    [{ GameStarted.GameId = gameId; PlayerCount = playerCount; FirstCard = firstCard }]


















let playCard gameId player card state : Event list =
    if player <> state.CurrentPlayer then invalidOp "Player should play at his turn"

    let reverse =
        function
        | ClockWise -> CounterClockWise
        | CounterClockWise -> ClockWise

    match card, state.TopCard with
    | Digit(n1, color1), Digit(n2, color2) when n1 = n2 || color1 = color2 ->
        [ {CardPlayed.GameId = gameId; Player = player; Card = card }]
    | KickBack(color1), Digit(_, color2) when color1 = color2 ->
        [ 
          {GameDirectionChanged.GameId=gameId; Direction = reverse state.Direction }
          {CardPlayed.GameId = gameId; Player = player; Card = card }
        ]

    | _ -> invalidOp "Play same color or same value !"















// Applies state changes for events

let apply (state: State) (event: Event) =
    match event with
    | :? GameStarted as e -> 
        { state with 
            GameId = e.GameId 
            GameAlreadyStarted = true
            PlayerCount = e.PlayerCount
            CurrentPlayer = 0
            TopCard = e.FirstCard
        }
    | :? CardPlayed as e ->
        { state with
            CurrentPlayer = 
                (state.CurrentPlayer + 1) % state.PlayerCount
            TopCard = e.Card
        }
    | :? GameDirectionChanged as e ->
        {
            state with
            Direction = e.Direction
        }
    | _ -> state

// Replays all events from start to get current state

let replay (events : Event list) =
    List.fold apply empty events



// Map commands to aggregates operations

let handle (command: Command) =
    match command with
    | :? StartGame as c -> startGame c.GameId c.PlayerCount c.FirstCard
    | :? PlayCard as c -> playCard c.GameId c.Player c.Card
    | _ -> invalidArg "command" "This command cannot be processed by discard pile"


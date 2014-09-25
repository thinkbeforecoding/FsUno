[<AutoOpen>]
module PrettyPrint

let printCard =
    function
    | Digit(n, color) -> sprintf "%A %d" color n
    | KickBack(color) -> sprintf "%A Kickback" color
  
let gameId = function GameId id -> id 
let printEvent (event: Event) =
    match event with
    | GameStarted e -> sprintf "Game %d started with %d players. Top Card is %s" (gameId e.GameId) e.PlayerCount (printCard e.FirstCard)
    | CardPlayed e -> sprintf "Player %d played %s" e.Player (printCard e.Card)

let printCommand (command: Command) =
    match command with
    | StartGame e -> sprintf "Start game %d with %d players. Top card %s" (gameId e.GameId) e.PlayerCount (printCard e.FirstCard)
    | PlayCard e -> sprintf "Player %d plays %s" e.Player (printCard e.Card)

let printGiven events =
    printfn "Given"
    events 
    |> List.map printEvent
    |> List.iter (printfn "\t%s")
   
let printWhen command =
    printfn "When"
    command |> printCommand  |> printfn "\t%s"

let printExpect events =
    printfn "Expect"
    events 
    |> List.map printEvent
    |> List.iter (printfn "\t%s")

let printExpectThrows ex =
    printfn "Expect"
    printfn "\t%A" ex    
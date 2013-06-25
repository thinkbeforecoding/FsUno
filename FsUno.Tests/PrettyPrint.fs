[<AutoOpen>]
module PrettyPrint

let printCard =
    function
    | Digit(n, color) -> sprintf "%A %d" color n
    
let printEvent (event: Event) =
    match event with
    | :? GameStarted as e -> sprintf "Game %d started with %d players. Top Card is %s" e.GameId e.PlayerCount (printCard e.FirstCard)
    | :? CardPlayed as e -> sprintf "Player %d played %s" e.Player (printCard e.Card)
    | _ -> ""

let printCommand (command: Command) =
    match command with
    | :? StartGame as c -> sprintf "Start game %d with %d players. Top card %s" c.GameId c.PlayerCount (printCard c.FirstCard)
    | :? PlayCard as e -> sprintf "Player %d plays %s" e.Player (printCard e.Card)
    | _ -> ""

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
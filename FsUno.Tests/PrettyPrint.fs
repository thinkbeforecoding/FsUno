[<AutoOpen>]
module PrettyPrint

let printCard =
    function
    | Digit(n, color) -> sprintf "%A %d" color n
    | KickBack(color) -> sprintf "%A Kickback" color
    
let printEvent (event: Event) =
    match event with
    | GameStarted(id, playerCount, firstCard) -> sprintf "Game %d started with %d players. Top Card is %s" id playerCount (printCard firstCard)
    | CardPlayed(id, player, card) -> sprintf "Player %d played %s" player (printCard card)

let printCommand (command: Command) =
    match command with
    | StartGame(id, playerCount, firstCard) -> sprintf "Start game %d with %d players. Top card %s" id playerCount (printCard firstCard)
    | PlayCard(id, player, card) -> sprintf "Player %d plays %s" player (printCard card)

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
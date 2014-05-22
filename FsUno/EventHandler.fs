namespace FsUno

open System

type EventHandler() =
    let mutable turnCount = 0

    let setColor card =
        let color = 
            match card with
            | Digit(_, c) -> Some c
            | KickBack c -> Some c
            |> function
               | Some Red -> ConsoleColor.Red
               | Some Green -> ConsoleColor.Green
               | Some Blue -> ConsoleColor.Blue
               | Some Yellow -> ConsoleColor.Yellow
               | None -> ConsoleColor.White
        Console.ForegroundColor <- color
    
    member this.Handle =
        function
        | GameStarted(id, playerCount, firstCard) ->
            printfn "Game %d started with %d players" id playerCount
            setColor firstCard
            printfn "First card: %A" firstCard

        | CardPlayed(id, player, card) ->
            turnCount <- turnCount + 1
            setColor card
            printfn "[%d] Player %d played %A" turnCount player card



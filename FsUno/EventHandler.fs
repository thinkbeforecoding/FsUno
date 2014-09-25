namespace FsUno

open System
open Game

type EventHandler() =
    let mutable turnCount = 0

    let gameId = function GameId id -> id

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
        | GameStarted e ->
            printfn "Game %d started with %d players" (gameId e.GameId) e.PlayerCount
            setColor e.FirstCard
            printfn "First card: %A" e.FirstCard

        | CardPlayed e ->
            turnCount <- turnCount + 1
            setColor e.Card
            printfn "[%d] Player %d played %A" turnCount e.Player e.Card



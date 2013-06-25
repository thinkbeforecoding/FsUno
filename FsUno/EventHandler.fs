namespace FsUno

open System

type EventHandler() =
    let mutable turnCount = 0

    let setColor card =
        let color = match card with
            | Digit(_, Red) -> ConsoleColor.Red
            | Digit(_, Green) -> ConsoleColor.Green
            | Digit(_, Blue) -> ConsoleColor.Blue
            | Digit(_, Yellow) -> ConsoleColor.Yellow
            | _ -> ConsoleColor.White

        Console.ForegroundColor <- color

    interface IPublisher with

        member this.Publish event =
            match event with
            | :? GameStarted as e ->
                printfn "Game %d started with %d players" e.GameId e.PlayerCount
                printfn "First card: %A" e.FirstCard

            | :? CardPlayed as e ->
                turnCount <- turnCount + 1
                setColor e.Card
                printfn "[%d] Player %d played %A" turnCount e.Player e.Card
            | _ -> ()



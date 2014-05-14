// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open FsUno

[<EntryPoint>]
let main argv = 

    //use store = new EventStore(new EventHandler())

    let store = new InMemoryEventStore(new EventHandler())

    let handle = DiscardPileCommandHandler.create store

    handle(StartGame(1, 4, Digit(3, Red)))
    
    handle(PlayCard(1, 0, Digit(3, Blue)))

    handle(PlayCard(1, 1, Digit(8, Blue)))
    
    handle(PlayCard(1, 2, Digit(8, Yellow)))
    
    handle(PlayCard(1, 3, Digit(4, Yellow)))
    
    handle(PlayCard(1, 0, Digit(4, Green)))


    System.Console.ReadLine() |> ignore

    0 // return an integer exit code

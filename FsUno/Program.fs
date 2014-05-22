// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open FsUno
open InMemoryEventStore
//open EventStore

[<EntryPoint>]
let main argv = 

    use store = create()

    let eventHandler = new EventHandler()

    let handle command = 
        command
        |> DiscardPileCommandHandler.create (readStream store) (appendToStream store)
        |> Seq.iter eventHandler.Handle

    handle (StartGame(1, 4, Digit(3, Red)))
    
    handle (PlayCard(1, 0, Digit(3, Blue)))

    handle (PlayCard(1, 1, Digit(8, Blue)))
    
    handle (PlayCard(1, 2, Digit(8, Yellow)))
    
    handle (PlayCard(1, 3, Digit(4, Yellow)))
    
    handle (PlayCard(1, 0, Digit(4, Green)))


    System.Console.ReadLine() |> ignore

    0 // return an integer exit code

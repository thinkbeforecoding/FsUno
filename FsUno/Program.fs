open FsUno
open CommandHandlers

open ToyInMemoryEventStore

// uncomment to use async agent version (against the event store)
// open Async 

// uncomment to use the EventStore 
// open EventStore


[<EntryPoint>]
let main _ = 
    
    let eventHandler = new EventHandler()
    use store = 
        create()
        |> subscribe eventHandler.Handle


    let handle = Game.create (readStream store) (appendToStream store)

    handle (StartGame(1, 4, Digit(3, Red)))
    
    handle (PlayCard(1, 0, Digit(3, Blue)))

    handle (PlayCard(1, 1, Digit(8, Blue)))
    
    handle (PlayCard(1, 2, Digit(8, Yellow)))
    
    handle (PlayCard(1, 3, Digit(4, Yellow)))
    
    handle (PlayCard(1, 0, Digit(4, Green)))


    System.Console.ReadLine() |> ignore

    0 // return an integer exit code

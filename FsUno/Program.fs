// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open FsUno

[<EntryPoint>]
let main argv = 

    //let store = new EventStore(new EventHandler())
    //store.Start()

    let store = new InMemoryEventStore(new EventHandler())

    let commandHandler = new DiscardPileCommandHandler(store)

    commandHandler.handle({StartGame.GameId = 1; PlayerCount = 4; FirstCard = Digit(3, Red)})
    
    commandHandler.handle({PlayCard.GameId = 1; Player = 0; Card = Digit(3, Blue)})

    commandHandler.handle({PlayCard.GameId = 1; Player = 1; Card = Digit(8, Blue)})
    
    commandHandler.handle({PlayCard.GameId = 1; Player = 2; Card = Digit(8, Yellow)})
    
    commandHandler.handle({PlayCard.GameId = 1; Player = 3; Card = Digit(4, Yellow)})
    
    commandHandler.handle({PlayCard.GameId = 1; Player = 0; Card = Digit(4, Green)})

    
    //store.Stop()

    0 // return an integer exit code

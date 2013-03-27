namespace FsUno

open DiscardPile

type DiscardPileCommandHandler(store: IEventStore) =
    
    let repository = new DiscardPileRepository(store)

    let handleBy id command =
        let version, state = repository.GetById id
        state 
        |> handle command
        |> repository.Save id version


    member this.handle (command: StartGame) =
        command
        |> handleBy command.GameId

    member this.handle (command: PlayCard) =
        command
        |> handleBy command.GameId
                
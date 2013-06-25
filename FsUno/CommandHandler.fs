namespace FsUno

open DiscardPile

type DiscardPileCommandHandler(store: IEventStore) =
    
    let repository = new DiscardPileRepository(store)

    let execute command on id =
        repository.GetById id
        %|> handle command
        ||> repository.Save id
    let on = ()

    member this.handle (command: StartGame) = execute command on command.GameId

    member this.handle (command: PlayCard) =  execute command on command.GameId
                
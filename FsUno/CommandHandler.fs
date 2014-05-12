namespace FsUno

open DiscardPile

type DiscardPileCommandHandler(store: IEventStore) =
    
    let repository = new DiscardPileRepository(store)

    member this.handle command =
        let id = gameId command

        repository.GetById id
        %|> handle command
        ||> repository.Save id

                
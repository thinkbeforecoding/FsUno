[<AutoOpen>]
module Commands

type Command =
    interface
    end

type StartGame =
    {
        GameId: GameId
        PlayerCount: int
        FirstCard: Card
    }
    interface Command

type PlayCard =
    {
        GameId: GameId
        Player: int
        Card: Card
    }
    interface Command

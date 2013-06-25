[<AutoOpen>]
module Events

type Event =
    interface
    end


[<CLIMutable>]
type GameStarted =
    {
        GameId: GameId
        PlayerCount: int
        FirstCard: Card
    }
    interface Event

[<CLIMutable>]
type CardPlayed = 
    { 
        GameId: GameId
        Player: int
        Card: Card
    }
    interface Event

[<CLIMutable>]
type GameDirectionChanged =
    {
        GameId: GameId
        Direction: Direction
    }
    interface Event












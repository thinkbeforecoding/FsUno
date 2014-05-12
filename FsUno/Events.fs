[<AutoOpen>]
module Events

type Event =
    | GameStarted of GameId: GameId * PlayerCount: int * FirstCard: Card
    | CardPlayed of GameId: GameId * Player: int * Card: Card
    | GameDirectionChanged of GameId: GameId * Direction: Direction












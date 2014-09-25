[<AutoOpen>]
module Events

type Event =
    | GameStarted of GameStarted 
    | CardPlayed of CardPlayed 

and GameStarted = {
    GameId: GameId
    PlayerCount: int
    FirstCard: Card }

and CardPlayed = {
    GameId: GameId
    Player: int
    Card: Card }
 

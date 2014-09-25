[<AutoOpen>]
module Commands

type Command =
    | StartGame of StartGame
    | PlayCard of PlayCard 

and StartGame = {
    GameId: GameId
    PlayerCount: int
    FirstCard: Card }

and PlayCard = {
    GameId: GameId
    Player: int
    Card: Card }


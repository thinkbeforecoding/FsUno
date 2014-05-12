[<AutoOpen>]
module Commands

type Command =
    | StartGame of GameId: GameId * PlayerCount: int * FirstCard: Card
    | PlayCard of GameId: GameId * Player: int * Card: Card

let gameId =
    function
    | StartGame(id, _, _) -> id
    | PlayCard(id, _, _) -> id
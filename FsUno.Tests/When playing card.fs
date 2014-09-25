module FsUno.Tests.``When playing card``

open Xunit
open System

[<Fact>]
let ``Same color should be accepted``() =
    Given [ GameStarted{ GameId = GameId 1; PlayerCount = 4; FirstCard = Digit(3, Red)} ]
    |> When ( PlayCard{ GameId = GameId 1; Player = 0; Card = Digit(9, Red)} )
    |> Expect [ CardPlayed{ GameId = GameId 1; Player = 0; Card = Digit(9, Red)} ]















[<Fact>]
let ``Same value should be accepted``() =
    Given [ GameStarted{ GameId = GameId 1; PlayerCount = 4; FirstCard = Digit(3, Red)} ]
    |> When ( PlayCard{ GameId = GameId 1; Player = 0; Card = Digit(3, Yellow)} )
    |> Expect [ CardPlayed{ GameId = GameId 1; Player = 0; Card = Digit(3, Yellow)} ]




[<Fact>]
let ``Different value and color should be rejected``() =
    Given [ GameStarted{ GameId = GameId 1; PlayerCount = 4; FirstCard = Digit(3, Red)} ]
    |> When ( PlayCard{ GameId = GameId 1; Player = 0; Card = Digit(8, Yellow)} )
    |> ExpectThrows<InvalidOperationException>

[<Fact>]
let ``First player should play at his turn``() =
    Given [ GameStarted{ GameId = GameId 1; PlayerCount = 4; FirstCard = Digit(3, Red)} ]
    |> When ( PlayCard{ GameId = GameId 1; Player = 2; Card = Digit(3, Green)} )
    |> ExpectThrows<InvalidOperationException>

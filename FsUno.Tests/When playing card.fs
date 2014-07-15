module FsUno.Tests.``When playing card``

open Xunit
open System

[<Fact>]
let ``Same color should be accepted``() =
    Given [ GameStarted(1, 4, Digit(3, Red)) ]
    |> When ( PlayCard(1, 0, Digit(9, Red)) )
    |> Expect [ CardPlayed(1, 0, Digit(9, Red)) ]















[<Fact>]
let ``Same value should be accepted``() =
    Given [ GameStarted(1, 4, Digit(3, Red)) ]
    |> When ( PlayCard(1, 0, Digit(3, Yellow)) )
    |> Expect [ CardPlayed(1, 0, Digit(3, Yellow)) ]




[<Fact>]
let ``Different value and color should be rejected``() =
    Given [ GameStarted(1, 4, Digit(3, Red)) ]
    |> When ( PlayCard(1, 0, Digit(8, Yellow)) )
    |> ExpectThrows<InvalidOperationException>

[<Fact>]
let ``First player should play at his turn``() =
    Given [ GameStarted(1, 4, Digit(3, Red)) ]
    |> When ( PlayCard(1, 2, Digit(3, Green)) )
    |> ExpectThrows<InvalidOperationException>

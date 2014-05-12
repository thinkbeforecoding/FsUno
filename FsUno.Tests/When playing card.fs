namespace FsUno.Tests

open NUnit.Framework
open System

[<TestFixture>]
module ``When playing card`` =
    

    [<Test>]
    let ``Same color should be accepted``() =
        Given [ GameStarted(1, 4, Digit(3, Red)) ] |>
        When ( PlayCard(1, 0, Digit(9, Red) ))                  |>
        Expect [ CardPlayed(1, 0, Digit(9, Red) ) ]















    [<Test>]
    let ``Same value should be accepted``() =
        Given [ GameStarted(1, 4, Digit(3, Red)) ] |>
        When ( PlayCard(1, 0, Digit(3, Yellow) ))                  |>
        Expect [ CardPlayed(1, 0, Digit(3, Yellow) ) ]




    [<Test>]
    let ``Different value and color should be rejected``() =
        Given [ GameStarted(1, 4, Digit(3, Red)) ] |>
        When ( PlayCard(1, 0, Digit(8, Yellow) ))                  |>
        ExpectThrows<InvalidOperationException>

    [<Test>]
    let ``First player should play at his turn``() =
        Given [ GameStarted(1, 4, Digit(3, Red)) ] |>
        When ( PlayCard(1, 2, Digit(3, Green) ))                |>
        ExpectThrows<InvalidOperationException>

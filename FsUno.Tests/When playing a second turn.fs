namespace FsUno.Tests

open NUnit.Framework
open System

[<TestFixture>]
module ``When playing a second turn`` =
    
    [<Test>]
    let ``Same color should be accepted``() =
        Given [ 
            GameStarted(1, 4, Digit(3, Red)) 
            CardPlayed(1, 0, Digit(9, Red))
            ] |>
        When ( PlayCard(1, 1, Digit(8, Red) ))                  |>
        Expect [
            CardPlayed(1, 1, Digit(8, Red) )
        ]

    [<Test>]
    let ``Same value should be accepted``() =
        Given [ 
            GameStarted(1, 4, Digit(3, Red)) 
            CardPlayed(1, 0, Digit(9, Red) )
            ] |>
        When ( PlayCard(1, 1, Digit(9, Yellow) ))                  |>
        Expect [
            CardPlayed(1, 1, Digit(9, Yellow) )
        ]

    [<Test>]
    let ``Different value and color should be rejected``() =
        Given [ 
            GameStarted(1, 4, Digit(3, Red)) 
            CardPlayed(1, 0, Digit(9, Red) )
            ] |>
        When ( PlayCard(1, 1, Digit(8, Yellow) ))                  |>
        ExpectThrows<InvalidOperationException>

    [<Test>]
    let ``First player should play at his turn``() =
        Given [ 
            GameStarted(1, 4, Digit(3, Red)) 
            CardPlayed(1, 0, Digit(9, Red) )
            ] |>
        When ( PlayCard(1, 2, Digit(9, Green) ))                |>
        ExpectThrows<InvalidOperationException>

    [<Test>]
    let ``After a full round it should be player 0 turn``() =
        Given [ 
            GameStarted(1, 4, Digit(3, Red)) 
            CardPlayed(1, 0, Digit(9, Red) )
            CardPlayed(1, 1, Digit(8, Red) )
            CardPlayed(1, 2, Digit(6, Red) )
            CardPlayed(1, 3, Digit(6, Red) )
            ] |>
        When ( PlayCard(1, 0, Digit(1, Red) ))                |>
        Expect [CardPlayed(1, 0, Digit(1, Red) )]

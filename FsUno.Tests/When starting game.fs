namespace FsUno.Tests


open NUnit.Framework
open FsUnit
open System

[<TestFixture>]
module ``When starting game`` = 
    


    [<Test>]
    let ``Started game should be started``() =
        Given []                                                                      |>
        When { StartGame.GameId = 1; PlayerCount = 4; FirstCard = Digit(3, Red)}      |>
        Expect [
            { GameStarted.GameId = 1; PlayerCount = 4; FirstCard = Digit(3, Red) }
        ]

    [<Test>]
    let ``0 players should be rejected``() =
        Given []                                                                      |>
        When { StartGame.GameId = 1; PlayerCount = 0; FirstCard = Digit(3, Red)}      |>
        ExpectThrows<ArgumentException>


    [<Test>]
    let ``Game should not be started twice``() =
        Given [{ GameStarted.GameId = 1; PlayerCount = 4; FirstCard = Digit(3, Red) }]                                                                      |>
        When { StartGame.GameId = 1; PlayerCount = 4; FirstCard = Digit(2, Red)}      |>
        ExpectThrows<InvalidOperationException>

namespace FsUno.Tests


open NUnit.Framework
open FsUnit

[<TestFixture>]
module ``When starting game`` = 
    


    [<Test>]
    let ``Started game should be started``() =
        Given []                                                                      |>
        When { StartGame.GameId = 1; PlayerCount = 4; FirstCard = Digit(3, Red)}      |>
        Expect [
            { GameStarted.GameId = 1; PlayerCount = 4; FirstCard = Digit(3, Red) }
        ]


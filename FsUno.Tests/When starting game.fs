[<TestFixture>]
module FsUno.Tests.``When starting game``

open System

[<Test>]
let ``Started game should be started``() =
    Given []
    |> When ( StartGame(1, 4, Digit(3, Red)))
    |> Expect [ GameStarted(1, 4, Digit(3, Red)) ]

[<Test>]
let ``0 players should be rejected``() =
    Given []
    |> When ( StartGame(1, 0, Digit(3, Red)) )
    |> ExpectThrows<ArgumentException>


[<Test>]
let ``Game should not be started twice``() =
    Given [GameStarted(1, 4, Digit(3, Red)) ]
    |> When ( StartGame(1, 4, Digit(2, Red)) )
    |> ExpectThrows<InvalidOperationException>

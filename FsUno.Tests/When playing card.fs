module FsUno.Tests.``When playing card``

open System
open Swensen.Unquote
open Xunit

let [<Fact>] ``Same color should be accepted`` () =
    <@ replay [ GameStarted(1, 4, Digit(3, Red)) ]
    |> handle ( PlayCard(1, 0, Digit(9, Red)) ) 
    =[ CardPlayed(1, 0, Digit(9, Red)) ] @> |> test

let [<Fact>] ``Same value should be accepted`` () =
    <@ replay [ GameStarted(1, 4, Digit(3, Red)) ]
    |> handle ( PlayCard(1, 0, Digit(3, Yellow)) )
    =[ CardPlayed(1, 0, Digit(3, Yellow)) ] @> |> test

let [<Fact>] ``Different value and color should be rejected`` () =
    <@ replay [ GameStarted(1, 4, Digit(3, Red)) ]
    |> handle ( PlayCard(1, 0, Digit(8, Yellow)) ) @>
    |> raises<InvalidOperationException>

let [<Fact>] ``First player should play at his turn``() =
    <@ replay [ GameStarted(1, 4, Digit(3, Red)) ]
    |> handle ( PlayCard(1, 2, Digit(3, Green)) ) @>
    |> raises<InvalidOperationException>
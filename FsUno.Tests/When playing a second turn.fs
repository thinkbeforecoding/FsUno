module FsUno.Tests.``When playing a second turn``

open System
open Swensen.Unquote
open Xunit

let [<Fact>] ``Same color should be accepted`` () =
    <@ replay [ GameStarted(1, 4, Digit(3, Red)) 
                CardPlayed(1, 0, Digit(9, Red)) ] 
    |> handle ( PlayCard(1, 1, Digit(8, Red)) ) 
    =[ CardPlayed(1, 1, Digit(8, Red)) ] @> |> test

let [<Fact>] ``Same value should be accepted`` () =
    <@ replay [ GameStarted(1, 4, Digit(3, Red)) 
                CardPlayed(1, 0, Digit(9, Red)) ] 
    |> handle ( PlayCard(1, 1, Digit(9, Yellow)) )
    =[ CardPlayed(1, 1, Digit(9, Yellow)) ] @> |> test

let [<Fact>] ``Different value and color should be rejected`` () =
    <@ replay [ GameStarted(1, 4, Digit(3, Red)) 
                CardPlayed(1, 0, Digit(9, Red)) ] 
    |> handle ( PlayCard(1, 1, Digit(8, Yellow)) ) @> 
    |> raises<InvalidOperationException>

let [<Fact>] ``First player should play at his turn``() =
    <@ replay [ GameStarted(1, 4, Digit(3, Red)) 
                CardPlayed(1, 0, Digit(9, Red)) ] 
    |> handle ( PlayCard(1, 4, Digit(9, Green)) ) @>
    |> raises<InvalidOperationException>

let [<Fact>] ``After a full round it should be player 0 turn``() =
    <@ replay [ GameStarted(1, 4, Digit(3, Red)) 
                CardPlayed(1, 0, Digit(9, Red))
                CardPlayed(1, 1, Digit(8, Red))
                CardPlayed(1, 2, Digit(6, Red))
                CardPlayed(1, 3, Digit(6, Red)) ] 
    |> handle ( PlayCard(1, 0, Digit(1, Red)) )                
    =[ CardPlayed(1, 0, Digit(1, Red)) ] @> |> test
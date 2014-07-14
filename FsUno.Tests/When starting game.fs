module FsUno.Tests.``When starting game``

let inline ( *> ) events command = handle command events
let inline (=>) left right = left = right

open Swensen.Unquote
open Xunit
open System

let [<Fact>] ``Started game should be started`` () = 
    <@ replay [ ] 
    |> handle ( StartGame(1, 3, Digit(3, Red)) )
    =[ GameStarted(1, 3, Digit(3, Red)) ] @> |> test

//  <@  replay  [ ] 
//  |>  handle  ( StartGame(1, 3, Digit(3, Red)) )
//  =           [ GameStarted(1, 3, Digit(3, Red)) ] @> |> test
//
//  test <@  replay [] 
//      |> handle (StartGame(1, 3, Digit(3, Red)))
//       = [GameStarted(1, 4, Digit(3, Red))] @> 
//
//  test <@ replay [] 
//          *> StartGame(1, 3, Digit(3, Red))
//          => [GameStarted(1, 4, Digit(3, Red))] @>
//
//  test |<
//  <@  replay [] 
//      *> StartGame(1, 3, Digit(3, Red))
//      => [GameStarted(1, 4, Digit(3, Red))] @>

let [<Fact>] ``0 players should be rejected`` () = 
    <@ replay [ ]
    |> handle ( StartGame(1, 0, Digit(3, Red)) ) @>
    |> raises<ArgumentException> 

let [<Fact>] ``Game should not be started twice`` () =
    <@ replay [ GameStarted(1, 4, Digit(3, Red)) ]
    |> handle ( StartGame(1, 4, Digit(2, Red)) ) @>
    |> raises<InvalidOperationException>
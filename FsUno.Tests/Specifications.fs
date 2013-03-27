[<AutoOpen>]
module Specifications

open NUnit.Framework
open FsUnit

let Given (events: Event list) = events
let When (command: Command) events = events, command
let Expect (expected: Event list) (events, command) =
    replay events
    |> handle command
    |> should equal expected

let ExpectThrows<'Ex> (events, command) =
    (fun () ->
        replay events
        |> handle command
        |> ignore)
    |> should throw typeof<'Ex>


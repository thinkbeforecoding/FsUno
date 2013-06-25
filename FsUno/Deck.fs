[<AutoOpen>]
module Deck

type GameId = int

type Color = 
    | Red
    | Green
    | Blue
    | Yellow

type Card =
    | Digit of int * Color
    | KickBack of Color

type Direction =
    | ClockWise
    | CounterClockWise

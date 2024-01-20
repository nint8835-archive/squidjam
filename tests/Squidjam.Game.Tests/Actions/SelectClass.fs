module Squidjam.Game.Tests.Actions.``Select Class``

open System
open NUnit.Framework

open Squidjam.Game

let invalidStates =
    [| Ended(None); PlayerTurn(0) |]
    |> Array.map (fun state -> TestCaseData(state))

[<Test>]
[<TestCaseSource("invalidStates")>]
let ``Invalid State`` (state: GameState) =
    let stateName = state.GetType().Name

    let initialGame =
        { Id = Guid.NewGuid()
          State = state
          Players =
            [| { Id = Guid.NewGuid()
                 Ready = true
                 Class = Some Grack } |] }

    let game =
        Actions.Apply initialGame (Actions.SelectClass(initialGame.Players[0], Grack))

    match game with
    | Ok g -> Assert.Fail($"Should not be able to select class in %s{stateName} state")
    | Error e -> Assert.AreEqual(e, $"Unable to select class in game state %s{stateName}")

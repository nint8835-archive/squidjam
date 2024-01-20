module Squidjam.Game.Tests.Actions.``End Turn``

open System
open NUnit.Framework

open Squidjam.Game

[<Test>]
let ``First Player`` () =
    let initialGame =
        { id = Guid.NewGuid()
          state = PlayerTurn(0)
          players =
            [| { id = Guid.NewGuid(); ready = true }
               { id = Guid.NewGuid(); ready = true } |] }

    let game = Actions.Apply initialGame (Actions.EndTurn initialGame.players[0])

    match game with
    | Ok g -> Assert.AreEqual(g, { initialGame with state = PlayerTurn(1) })
    | Error e -> Assert.Fail(e)

[<Test>]
let ``Last Player`` () =
    let initialGame =
        { id = Guid.NewGuid()
          state = PlayerTurn(1)
          players =
            [| { id = Guid.NewGuid(); ready = true }
               { id = Guid.NewGuid(); ready = true } |] }

    let game = Actions.Apply initialGame (Actions.EndTurn initialGame.players[1])

    match game with
    | Ok g -> Assert.AreEqual(g, { initialGame with state = PlayerTurn(0) })
    | Error e -> Assert.Fail(e)

[<Test>]
let ``Not Player's Turn`` () =
    let initialGame =
        { id = Guid.NewGuid()
          state = PlayerTurn(1)
          players =
            [| { id = Guid.NewGuid(); ready = true }
               { id = Guid.NewGuid(); ready = true } |] }

    let game = Actions.Apply initialGame (Actions.EndTurn initialGame.players[0])

    match game with
    | Ok g -> Assert.Fail("Should not be able to end turn when it is not your turn")
    | Error e -> Assert.AreEqual(e, "You cannot end the turn when it is not your turn")

let invalidStates =
    [| Ended(None); PlayerRegistration |]
    |> Array.map (fun state -> TestCaseData(state))

[<Test>]
[<TestCaseSource("invalidStates")>]
let ``Invalid State`` (state: GameState) =
    let stateName = state.GetType().Name

    let initialGame =
        { id = Guid.NewGuid()
          state = state
          players = [| { id = Guid.NewGuid(); ready = true } |] }

    let game = Actions.Apply initialGame (Actions.EndTurn initialGame.players[0])

    match game with
    | Ok g -> Assert.Fail($"Should not be able to end turn in %s{stateName} state")
    | Error e -> Assert.AreEqual(e, $"Unable to end turn in game state %s{stateName}")

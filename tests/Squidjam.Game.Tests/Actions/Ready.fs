module Squidjam.Game.Tests.Actions.Ready

open System
open NUnit.Framework

open Squidjam.Game

[<Test>]
let ``Ready Player`` () =
    let initialGame =
        { id = Guid.NewGuid()
          state = PlayerRegistration
          players =
            [| { id = Guid.NewGuid(); ready = false }
               { id = Guid.NewGuid(); ready = false } |] }

    let game = Actions.Apply initialGame (Actions.Ready initialGame.players[0])

    match game with
    | Ok g ->
        Assert.AreEqual(g.players[0], { initialGame.players[0] with ready = true })
        Assert.AreEqual(g.state, PlayerRegistration)
    | Error e -> Assert.Fail(e)

[<Test>]
let ``Single Player Readying Doesn't Start Game`` () =
    let initialGame =
        { id = Guid.NewGuid()
          state = PlayerRegistration
          players = [| { id = Guid.NewGuid(); ready = false } |] }

    let game = Actions.Apply initialGame (Actions.Ready initialGame.players[0])

    match game with
    | Ok g ->
        Assert.AreEqual(g.players[0], { initialGame.players[0] with ready = true })
        Assert.AreEqual(g.state, PlayerRegistration)
    | Error e -> Assert.Fail(e)

[<Test>]
let ``All Players Readying Starts Game`` () =
    let initialGame =
        { id = Guid.NewGuid()
          state = PlayerRegistration
          players =
            [| { id = Guid.NewGuid(); ready = false }
               { id = Guid.NewGuid(); ready = true } |] }

    let game = Actions.Apply initialGame (Actions.Ready initialGame.players[0])

    match game with
    | Ok g ->
        Assert.AreEqual(g.players[0], { initialGame.players[0] with ready = true })
        Assert.AreEqual(g.state, PlayerTurn(0))
    | Error e -> Assert.Fail(e)


let invalidStates =
    [| Ended(None); PlayerTurn(0) |]
    |> Array.map (fun state -> TestCaseData(state))

[<Test>]
[<TestCaseSource("invalidStates")>]
let ``Invalid State`` (state: GameState) =
    let stateName = state.GetType().Name

    let initialGame =
        { id = Guid.NewGuid()
          state = state
          players = [| { id = Guid.NewGuid(); ready = true } |] }

    let game = Actions.Apply initialGame (Actions.Ready initialGame.players[0])

    match game with
    | Ok g -> Assert.Fail($"Should not be able to ready player in %s{stateName} state")
    | Error e -> Assert.AreEqual(e, $"Unable to ready player in game state %s{stateName}")

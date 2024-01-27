module Squidjam.Game.Tests.Actions.Ready

open System
open NUnit.Framework

open Squidjam.Game

[<Test>]
let ``Ready Player`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerRegistration
          Players =
            [| { Id = Guid.NewGuid()
                 Ready = false
                 Class = Some Grack }
               { Id = Guid.NewGuid()
                 Ready = false
                 Class = Some Grack } |] }

    let game = Actions.Apply initialGame (Actions.Ready initialGame.Players[0].Id)

    match game with
    | Ok g ->
        Assert.AreEqual(g.Players[0], { initialGame.Players[0] with Ready = true })
        Assert.AreEqual(g.State, PlayerRegistration)
    | Error e -> Assert.Fail(e)

[<Test>]
let ``Single Player Readying Doesn't Start Game`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerRegistration
          Players =
            [| { Id = Guid.NewGuid()
                 Ready = false
                 Class = Some Grack } |] }

    let game = Actions.Apply initialGame (Actions.Ready initialGame.Players[0].Id)

    match game with
    | Ok g ->
        Assert.AreEqual(g.Players[0], { initialGame.Players[0] with Ready = true })
        Assert.AreEqual(g.State, PlayerRegistration)
    | Error e -> Assert.Fail(e)

[<Test>]
let ``All Players Readying Starts Game`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerRegistration
          Players =
            [| { Id = Guid.NewGuid()
                 Ready = false
                 Class = Some Grack }
               { Id = Guid.NewGuid()
                 Ready = true
                 Class = Some Grack } |] }

    let game = Actions.Apply initialGame (Actions.Ready initialGame.Players[0].Id)

    match game with
    | Ok g ->
        Assert.AreEqual(g.Players[0], { initialGame.Players[0] with Ready = true })
        Assert.AreEqual(g.State, PlayerTurn(0))
    | Error e -> Assert.Fail(e)

[<Test>]
let ``Cannot Ready Without Class Selected`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerRegistration
          Players =
            [| { Id = Guid.NewGuid()
                 Ready = true
                 Class = None } |] }

    let game = Actions.Apply initialGame (Actions.Ready initialGame.Players[0].Id)

    match game with
    | Ok g -> Assert.Fail("Should not be able to ready player without class selected")
    | Error e -> Assert.AreEqual(e, "You must select a class before you can ready")


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

    let game = Actions.Apply initialGame (Actions.Ready initialGame.Players[0].Id)

    match game with
    | Ok g -> Assert.Fail($"Should not be able to ready player in %s{stateName} state")
    | Error e -> Assert.AreEqual(e, $"Unable to ready player in game state %s{stateName}")

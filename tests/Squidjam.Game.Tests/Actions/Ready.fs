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
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = Some Grack
                 Creatures = [||]
                 MutationDeck = [||]
                 MutationHand = [||] }
               { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = Some Grack
                 Creatures = [||]
                 MutationDeck = [||]
                 MutationHand = [||] } |] }

    let game = Actions.Apply initialGame (Actions.Ready initialGame.Players[0].Id)

    match game with
    | Ok g ->
        Assert.AreEqual(
            g.Players[0],
            { initialGame.Players[0] with
                Ready = true }
        )

        Assert.AreEqual(g.State, PlayerRegistration)
    | Error e -> Assert.Fail(e)

[<Test>]
let ``Single Player Readying Doesn't Start Game`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerRegistration
          Players =
            [| { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = Some Grack
                 Creatures = [||]
                 MutationDeck = [||]
                 MutationHand = [||] } |] }

    let game = Actions.Apply initialGame (Actions.Ready initialGame.Players[0].Id)

    match game with
    | Ok g ->
        Assert.AreEqual(
            g.Players[0],
            { initialGame.Players[0] with
                Ready = true }
        )

        Assert.AreEqual(g.State, PlayerRegistration)
    | Error e -> Assert.Fail(e)

[<Test>]
let ``All Players Readying Starts Game`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerRegistration
          Players =
            [| { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = Some Grack
                 Creatures = [||]
                 MutationDeck = [||]
                 MutationHand = [||] }
               { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = true
                 Class = Some Grack
                 Creatures = [||]
                 MutationDeck = [||]
                 MutationHand = [||] } |] }

    let game = Actions.Apply initialGame (Actions.Ready initialGame.Players[0].Id)

    match game with
    | Ok g ->
        Assert.AreEqual(
            g.Players[0],
            { initialGame.Players[0] with
                Ready = true }
        )

        Assert.AreEqual(g.State, PlayerTurn(0))
    | Error e -> Assert.Fail(e)

[<Test>]
let ``Cannot Ready Without Class Selected`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerRegistration
          Players =
            [| { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = true
                 Class = None
                 Creatures = [||]
                 MutationDeck = [||]
                 MutationHand = [||] } |] }

    let game = Actions.Apply initialGame (Actions.Ready initialGame.Players[0].Id)

    match game with
    | Ok g -> Assert.Fail("Should not be able to ready player without class selected")
    | Error e -> Assert.AreEqual(e, "You must select a class before you can ready")

[<Test>]
let ``Cannot Ready If Not In Game`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerRegistration
          Players = [||] }

    let game = Actions.Apply initialGame (Actions.Ready(Guid.NewGuid()))

    match game with
    | Ok g -> Assert.Fail("Should not be able to ready player not in game")
    | Error e -> Assert.AreEqual(e, "You are not in this game")


let invalidStates =
    [| Ended(None); PlayerTurn(0) |] |> Array.map (fun state -> TestCaseData(state))

[<Test>]
[<TestCaseSource("invalidStates")>]
let ``Invalid State`` (state: GameState) =
    let stateName = state.GetType().Name

    let initialGame =
        { Id = Guid.NewGuid()
          State = state
          Players =
            [| { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = true
                 Class = Some Grack
                 Creatures = [||]
                 MutationDeck = [||]
                 MutationHand = [||] } |] }

    let game = Actions.Apply initialGame (Actions.Ready initialGame.Players[0].Id)

    match game with
    | Ok g -> Assert.Fail($"Should not be able to ready player in %s{stateName} state")
    | Error e -> Assert.AreEqual(e, $"Unable to ready player in game state %s{stateName}")

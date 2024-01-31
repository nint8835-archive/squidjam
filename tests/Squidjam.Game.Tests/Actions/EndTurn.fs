module Squidjam.Game.Tests.Actions.``End Turn``

open System
open NUnit.Framework

open Squidjam.Game

[<Test>]
let ``First Player`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerTurn(0)
          Players =
            [| { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = true
                 Class = Some Grack
                 Creatures = Creatures.ClassCreatures[Grack]
                 RemainingEnergy = 0
                 MaxEnergy = 0
                 MutationDeck = [||]
                 MutationHand = [||] }
               { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = true
                 Class = Some Grack
                 Creatures = Creatures.ClassCreatures[Grack]
                 RemainingEnergy = 0
                 MaxEnergy = 0
                 MutationDeck = [||]
                 MutationHand = [||] } |] }

    let game = Actions.Apply initialGame (Actions.EndTurn initialGame.Players[0].Id)

    match game with
    | Ok g ->
        Assert.AreEqual(
            g,
            { initialGame with
                State = PlayerTurn(1)
                Players =
                    [| { initialGame.Players[0] with
                           MutationHand = [| Mutations.GrackSticker |]
                           MutationDeck = [||] }
                       initialGame.Players[1] |] }
        )
    | Error e -> Assert.Fail(e)

[<Test>]
let ``Last Player`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerTurn(1)
          Players =
            [| { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = true
                 Class = Some Grack
                 Creatures = [||]
                 RemainingEnergy = 0
                 MaxEnergy = 0
                 MutationDeck = [||]
                 MutationHand = [||] }
               { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = true
                 Class = Some Grack
                 Creatures = [||]
                 RemainingEnergy = 0
                 MaxEnergy = 0
                 MutationDeck = [||]
                 MutationHand = [||] } |] }

    let game = Actions.Apply initialGame (Actions.EndTurn initialGame.Players[1].Id)

    match game with
    | Ok g ->
        Assert.AreEqual(
            g,
            { initialGame with
                State = PlayerTurn(0)
                Players =
                    [| initialGame.Players[0]
                       { initialGame.Players[1] with
                           MutationHand = [| Mutations.GrackSticker |]
                           MutationDeck = [||] } |] }
        )
    | Error e -> Assert.Fail(e)

[<Test>]
let ``Not Player's Turn`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerTurn(1)
          Players =
            [| { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = true
                 Class = Some Grack
                 Creatures = [||]
                 RemainingEnergy = 0
                 MaxEnergy = 0
                 MutationDeck = [||]
                 MutationHand = [||] }
               { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = true
                 Class = Some Grack
                 Creatures = [||]
                 RemainingEnergy = 0
                 MaxEnergy = 0
                 MutationDeck = [||]
                 MutationHand = [||] } |] }

    let game = Actions.Apply initialGame (Actions.EndTurn initialGame.Players[0].Id)

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
        { Id = Guid.NewGuid()
          State = state
          Players =
            [| { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = true
                 Class = Some Grack
                 Creatures = [||]
                 RemainingEnergy = 0
                 MaxEnergy = 0
                 MutationDeck = [||]
                 MutationHand = [||] } |] }

    let game = Actions.Apply initialGame (Actions.EndTurn initialGame.Players[0].Id)

    match game with
    | Ok g -> Assert.Fail($"Should not be able to end turn in %s{stateName} state")
    | Error e -> Assert.AreEqual(e, $"Unable to end turn in game state %s{stateName}")

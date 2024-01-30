module Squidjam.Game.Tests.Actions.``Select Class``

open System
open NUnit.Framework

open Squidjam.Game

[<Test>]
let ``With No Current Class`` () =
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

    let game =
        Actions.Apply initialGame (Actions.SelectClass(initialGame.Players[0].Id, Grack))

    match game with
    | Ok g ->
        Assert.AreEqual(g.Players[0].Class, Some Grack)
        Assert.AreEqual(g.Players[0].Creatures, Creatures.ClassCreatures[Grack])
        Assert.AreEqual(g.Players[0].MutationDeck, Mutations.ClassMutations[Grack])
    | Error e -> Assert.Fail(e)

[<Test>]
let ``With Current Class`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerRegistration
          Players =
            [| { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = true
                 Class = Some Grack
                 Creatures = Creatures.ClassCreatures[Grack]
                 MutationDeck = Mutations.ClassMutations[Grack]
                 MutationHand = [||] } |] }

    let game =
        Actions.Apply initialGame (Actions.SelectClass(initialGame.Players[0].Id, Gump))

    match game with
    | Ok g ->
        Assert.AreEqual(g.Players[0].Class, Some Gump)
        Assert.AreEqual(g.Players[0].Creatures, Creatures.ClassCreatures[Gump])
        Assert.AreEqual(g.Players[0].MutationDeck, Mutations.ClassMutations[Gump])
    | Error e -> Assert.Fail(e)

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

    let game =
        Actions.Apply initialGame (Actions.SelectClass(initialGame.Players[0].Id, Grack))

    match game with
    | Ok g -> Assert.Fail($"Should not be able to select class in %s{stateName} state")
    | Error e -> Assert.AreEqual(e, $"Unable to select class in game state %s{stateName}")

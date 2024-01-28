module Squidjam.Game.Tests.Actions.``Remove Player``

open System
open NUnit.Framework
open Squidjam.Game

[<Test>]
let ``Player Not In Game`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerRegistration
          Players =
            [| { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = None
                 Creatures = [||] } |] }

    let game = Actions.Apply initialGame (Actions.RemovePlayer(Guid.NewGuid()))

    match game with
    | Ok g -> Assert.Fail("Should not be able to remove player not in game")
    | Error e -> Assert.AreEqual(e, "You are not in this game")


[<Test>]
let ``Remove Player Who's Turn It Is - Beginning of Turn Order`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerTurn(0)
          Players =
            [| { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = None
                 Creatures = [||] }
               { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = None
                 Creatures = [||] }
               { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = None
                 Creatures = [||] } |] }

    let game =
        Actions.Apply initialGame (Actions.RemovePlayer(initialGame.Players[0].Id))

    match game with
    | Ok g ->
        Assert.AreEqual(
            g,
            { initialGame with
                Players =
                    [| initialGame.Players[1]
                       initialGame.Players[2] |] }
        )
    | Error e -> Assert.Fail(e)

[<Test>]
let ``Remove Player Who's Turn It Is - Middle of Turn Order`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerTurn(1)
          Players =
            [| { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = None
                 Creatures = [||] }
               { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = None
                 Creatures = [||] }
               { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = None
                 Creatures = [||] } |] }

    let game =
        Actions.Apply initialGame (Actions.RemovePlayer(initialGame.Players[1].Id))

    match game with
    | Ok g ->
        Assert.AreEqual(
            g,
            { initialGame with
                Players =
                    [| initialGame.Players[0]
                       initialGame.Players[2] |] }
        )
    | Error e -> Assert.Fail(e)

[<Test>]
let ``Remove Player Who's Turn It Is - End of Turn Order`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerTurn(2)
          Players =
            [| { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = None
                 Creatures = [||] }
               { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = None
                 Creatures = [||] }
               { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = None
                 Creatures = [||] } |] }

    let game =
        Actions.Apply initialGame (Actions.RemovePlayer(initialGame.Players[2].Id))

    match game with
    | Ok g ->
        Assert.AreEqual(
            g,
            { initialGame with
                State = PlayerTurn(0)
                Players =
                    [| initialGame.Players[0]
                       initialGame.Players[1] |] }
        )
    | Error e -> Assert.Fail(e)

[<Test>]
let ``Remove Last Opponent`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerTurn(0)
          Players =
            [| { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = None
                 Creatures = [||] }
               { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = None
                 Creatures = [||] } |] }

    let game =
        Actions.Apply initialGame (Actions.RemovePlayer(initialGame.Players[1].Id))

    match game with
    | Ok g ->
        Assert.AreEqual(
            g,
            { initialGame with
                // TODO: Once game ending is implemented, re-compute the winner when a player leaves and transition the state if so
                // State = Ended(0)
                Players = [| initialGame.Players[0] |] }
        )
    | Error e -> Assert.Fail(e)

[<Test>]
let ``Remove Last Player`` () =
    let initialGame =
        { Id = Guid.NewGuid()
          State = PlayerTurn(0)
          Players =
            [| { Id = Guid.NewGuid()
                 Name = Guid.NewGuid().ToString()
                 Ready = false
                 Class = None
                 Creatures = [||] } |] }

    let game =
        Actions.Apply initialGame (Actions.RemovePlayer(initialGame.Players[0].Id))

    match game with
    | Ok g ->
        Assert.AreEqual(
            g,
            { initialGame with
                State = Ended(None)
                Players = [||] }
        )
    | Error e -> Assert.Fail(e)

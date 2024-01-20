module Squidjam.Game.Tests.Actions.``End Turn``

open System
open NUnit.Framework

open Squidjam.Game

[<Test>]
let ``First Player`` () =
    let initialGame =
        { state = PlayerTurn(0)
          players =
            [| { id = Guid.NewGuid() }
               { id = Guid.NewGuid() } |] }

    let game = Actions.Apply initialGame Actions.EndTurn

    match game with
    | Ok g -> Assert.AreEqual(g, { initialGame with state = PlayerTurn(1) })
    | Error e -> Assert.Fail(e)

[<Test>]
let ``Last Player`` () =
    let initialGame =
        { state = PlayerTurn(1)
          players =
            [| { id = Guid.NewGuid() }
               { id = Guid.NewGuid() } |] }

    let game = Actions.Apply initialGame Actions.EndTurn

    match game with
    | Ok g -> Assert.AreEqual(g, { initialGame with state = PlayerTurn(0) })
    | Error e -> Assert.Fail(e)

[<Test>]
let ``Invalid State`` () =
    let initialGame = { state = Ended(None); players = [||] }

    let game = Actions.Apply initialGame Actions.EndTurn

    match game with
    | Ok g -> Assert.Fail("Should not be able to end turn in Ended state")
    | Error e -> Assert.AreEqual(e, "Unable to end turn in game state Ended")

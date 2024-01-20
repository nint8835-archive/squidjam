module Squidjam.Game.Tests.GameUtils

open System
open NUnit.Framework

open Squidjam.Game

let exampleGame =
    { id = Guid.NewGuid()
      state = PlayerTurn 0
      players =
        [| { id = Guid.NewGuid(); ready = true }
           { id = Guid.NewGuid(); ready = true } |] }

[<Test>]
let GetPlayerById () =
    let secondPlayer = exampleGame.players[1]

    let player = GameUtils.GetPlayerById exampleGame secondPlayer.id
    Assert.AreEqual(secondPlayer, player)

[<Test>]
let GetPlayerIndex () =
    let secondPlayer = exampleGame.players[1]

    let index = GameUtils.GetPlayerIndex exampleGame secondPlayer.id
    Assert.AreEqual(1, index)

[<Test>]
let UpdatePlayer () =
    let secondPlayer = exampleGame.players.[1]

    let updatedGame =
        GameUtils.UpdatePlayer exampleGame secondPlayer.id (fun p -> { p with ready = false })

    Assert.AreEqual(updatedGame.players[1], { secondPlayer with ready = false })

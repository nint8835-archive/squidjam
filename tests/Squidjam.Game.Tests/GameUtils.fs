module Squidjam.Game.Tests.GameUtils

open System
open NUnit.Framework

open Squidjam.Game

let exampleGame =
    { Id = Guid.NewGuid()
      State = PlayerTurn 0
      Players =
        [| { Id = Guid.NewGuid(); Ready = true }
           { Id = Guid.NewGuid(); Ready = true } |] }

[<Test>]
let GetPlayerById () =
    let secondPlayer = exampleGame.Players[1]

    let player = GameUtils.GetPlayerById exampleGame secondPlayer.Id
    Assert.AreEqual(secondPlayer, player)

[<Test>]
let GetPlayerIndex () =
    let secondPlayer = exampleGame.Players[1]

    let index = GameUtils.GetPlayerIndex exampleGame secondPlayer.Id
    Assert.AreEqual(1, index)

[<Test>]
let UpdatePlayer () =
    let secondPlayer = exampleGame.Players.[1]

    let updatedGame =
        GameUtils.UpdatePlayer exampleGame secondPlayer.Id (fun p -> { p with Ready = false })

    Assert.AreEqual(updatedGame.Players[1], { secondPlayer with Ready = false })

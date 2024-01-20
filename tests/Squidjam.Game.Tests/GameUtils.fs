module Squidjam.Game.Tests.GameUtils

open System
open NUnit.Framework

open Squidjam.Game

let exampleGame =
    { id = Guid.NewGuid()
      state = PlayerTurn 0
      players =
        [| { id = Guid.NewGuid() }
           { id = Guid.NewGuid() } |] }

[<Test>]
let GetPlayerById () =
    let secondPlayer = exampleGame.players[1]

    let player = GameUtils.GetPlayerById exampleGame secondPlayer.id
    Assert.AreEqual(secondPlayer, player)

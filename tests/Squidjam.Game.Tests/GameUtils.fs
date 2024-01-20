module Squidjam.Game.Tests.GameUtils

open System
open NUnit.Framework

open Squidjam.Game

let player1Id = Guid.NewGuid()
let player2Id = Guid.NewGuid()

let exampleGame =
    { state = PlayerTurn 0
      players =
        Map [ (player1Id, { id = player1Id })
              (player2Id, { id = player2Id }) ] }

[<Test>]
let GetPlayerByIndex () =
    let secondPlayer =
        exampleGame.players
        |> Map.values
        |> Seq.skip 1
        |> Seq.head

    let player = GameUtils.GetPlayerByIndex exampleGame 1
    Assert.AreEqual(secondPlayer, player)

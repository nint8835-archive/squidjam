module Squidjam.Game.Tests.GameUtils

open System
open NUnit.Framework

open Squidjam.Game

let exampleGame =
    { Id = Guid.NewGuid()
      State = PlayerTurn 0
      Players =
        [| { Id = Guid.NewGuid()
             Name = Guid.NewGuid().ToString()
             Ready = true
             Class = Some Grack
             Creatures = [||] }
           { Id = Guid.NewGuid()
             Name = Guid.NewGuid().ToString()
             Ready = true
             Class = Some Grack
             Creatures = [||] } |] }

[<Test>]
let GetPlayerById () =
    let secondPlayer = exampleGame.Players[1]

    let player = GameUtils.GetPlayerById exampleGame secondPlayer.Id

    match player with
    | None -> Assert.Fail("Expected to get player")
    | Some p -> Assert.AreEqual(secondPlayer, p)

[<Test>]
let ``GetPlayerById - Not In Game`` () =
    let player = GameUtils.GetPlayerById exampleGame (Guid.NewGuid())

    match player with
    | None -> ()
    | Some _ -> Assert.Fail("Expected to not get player")

[<Test>]
let GetPlayerIndex () =
    let secondPlayer = exampleGame.Players[1]

    let index = GameUtils.GetPlayerIndex exampleGame secondPlayer.Id
    Assert.AreEqual(1, index)

[<Test>]
let UpdatePlayer () =
    let secondPlayer = exampleGame.Players.[1]

    let updatedGame =
        exampleGame
        |> GameUtils.UpdatePlayer secondPlayer.Id (fun p -> { p with Ready = false })

    Assert.AreEqual(updatedGame.Players[1], { secondPlayer with Ready = false })

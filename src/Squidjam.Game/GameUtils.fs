module Squidjam.Game.GameUtils

open System

let GetPlayerById (game: Game) (id: Guid) : Player =
    game.Players |> Array.find (fun p -> p.Id = id)

let GetPlayerIndex (game: Game) (id: Guid) : int =
    game.Players
    |> Array.findIndex (fun p -> p.Id = id)

let UpdatePlayer (game: Game) (id: Guid) (f: Player -> Player) : Game =
    let index = GetPlayerIndex game id
    let player = game.Players[index]
    let updatedPlayer = f player
    let updatedPlayers = game.Players |> Array.updateAt index updatedPlayer
    { game with Players = updatedPlayers }

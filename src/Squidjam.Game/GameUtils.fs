module Squidjam.Game.GameUtils

open System

let GetPlayerById (game: Game) (id: Guid) : Player =
    game.players |> Array.find (fun p -> p.id = id)

let GetPlayerIndex (game: Game) (id: Guid) : int =
    game.players
    |> Array.findIndex (fun p -> p.id = id)

let UpdatePlayer (game: Game) (id: Guid) (f: Player -> Player) : Game =
    let index = GetPlayerIndex game id
    let player = game.players[index]
    let updatedPlayer = f player
    let updatedPlayers = game.players |> Array.updateAt index updatedPlayer
    { game with players = updatedPlayers }

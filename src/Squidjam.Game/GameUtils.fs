module Squidjam.Game.GameUtils

open System

let GetPlayerById (game: Game) (id: Guid) : Player =
    game.players |> Array.find (fun p -> p.id = id)

let GetPlayerIndex (game: Game) (id: Guid) : int =
    game.players
    |> Array.findIndex (fun p -> p.id = id)

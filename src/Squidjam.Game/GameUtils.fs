module Squidjam.Game.GameUtils

open System

let GetPlayerById (game: Game) (id: Guid) : Player =
    game.players |> Array.find (fun p -> p.id = id)

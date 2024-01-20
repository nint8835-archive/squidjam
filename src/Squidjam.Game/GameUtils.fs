module Squidjam.Game.GameUtils

let GetPlayerByIndex (game: Game) (index: int) =
    game.players
    |> Map.toList
    |> List.item index
    |> snd

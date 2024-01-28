module Squidjam.Game.GameUtils

open System
open System.Security.Cryptography

let GetPlayerById (game: Game) (id: Guid) : Player option =
    game.Players |> Array.tryFind (fun p -> p.Id = id)

let GetPlayerIndex (game: Game) (id: Guid) : int =
    game.Players
    |> Array.findIndex (fun p -> p.Id = id)

let UpdatePlayer (game: Game) (id: Guid) (f: Player -> Player) : Game =
    let index = GetPlayerIndex game id
    let player = game.Players[index]
    let updatedPlayer = f player
    let updatedPlayers = game.Players |> Array.updateAt index updatedPlayer
    { game with Players = updatedPlayers }

// TODO: Test
let DeterministicShuffle (seeds: Guid array) array =
    let seed =
            MD5
                .Create()
                .ComputeHash(
                    seeds |> Array.map (_.ToByteArray()) |> Array.concat
                )
            |> BitConverter.ToInt32
    
    let random = Random(seed)
    
    let clonedArray = Array.copy array
    random.Shuffle clonedArray
    
    clonedArray
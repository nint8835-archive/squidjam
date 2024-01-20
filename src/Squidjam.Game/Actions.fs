module Squidjam.Game.Actions

open System
open System.Security.Cryptography

type Action =
    | EndTurn of Player
    | AddPlayer of Player
    | Ready of Player
    | SelectClass of Player * Class

let endTurn (game: Game) (player: Player) : Result<Game, string> =
    match game.State with
    | PlayerTurn playerIndex ->
        if GameUtils.GetPlayerIndex game player.Id = playerIndex then
            Ok { game with State = PlayerTurn((playerIndex + 1) % game.Players.Length) }
        else
            Error "You cannot end the turn when it is not your turn"
    | _ -> Error $"Unable to end turn in game state %s{game.State.GetType().Name}"

let addPlayer (game: Game) (player: Player) : Result<Game, string> =
    if game.State <> PlayerRegistration then
        Error $"Unable to add player in game state %s{game.State.GetType().Name}"
    else
        let playerSeed =
            MD5
                .Create()
                .ComputeHash(
                    game.Id.ToByteArray()
                    |> Array.append (player.Id.ToByteArray())
                )
            |> BitConverter.ToInt32

        let random = Random(playerSeed)

        let newPlayerArray = Array.append game.Players [| player |]

        random.Shuffle newPlayerArray

        Ok { game with Players = newPlayerArray }

let ready (game: Game) (player: Player) : Result<Game, string> =
    if game.State <> PlayerRegistration then
        Error $"Unable to ready player in game state %s{game.State.GetType().Name}"
    else if player.Class.IsNone then
        Error "You must select a class before you can ready"
    else
        let updatedGame = GameUtils.UpdatePlayer game player.Id (fun p -> { p with Ready = true })
        
        if updatedGame.Players |> Array.forall (_.Ready) && updatedGame.Players.Length > 1 then
            Ok { updatedGame with State = PlayerTurn 0 }
        else
            Ok updatedGame

let selectClass (game: Game) (player: Player) (newClass: Class) : Result<Game, String> =
    if game.State <> PlayerRegistration then
        Error $"Unable to select class in game state %s{game.State.GetType().Name}"
    else
        let updatedGame = GameUtils.UpdatePlayer game player.Id (fun p -> { p with Class = Some newClass })

        Ok updatedGame


let Apply (game: Game) (action: Action) : Result<Game, string> =
    match action with
    | EndTurn player -> endTurn game player
    | AddPlayer player -> addPlayer game player
    | Ready player -> ready game player
    | SelectClass (player, newClass) -> selectClass game player newClass
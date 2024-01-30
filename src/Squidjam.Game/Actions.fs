module Squidjam.Game.Actions

open System

let checkWinState (game: Game) : Game =
    let alivePlayers = game.Players |> Array.filter (fun p -> p.Creatures.Length > 0)

    if alivePlayers.Length = 1 then
        { game with
            State = Ended(Some alivePlayers.[0].Id) }
    else if alivePlayers.Length = 0 then
        { game with State = Ended(None) }
    else
        game

type Action =
    | EndTurn of Player: Guid
    | AddPlayer of Player: Guid * Name: string
    | RemovePlayer of Player: Guid
    | Ready of Player: Guid
    | SelectClass of Player: Guid * Class: Class
    | Attack of Player: Guid * AttackingCreatureIndex: int * TargetPlayer: Guid * TargetCreatureIndex: int

let endTurn (game: Game) (player: Guid) : Result<Game, string> =
    match game.State with
    | PlayerTurn playerIndex ->
        if GameUtils.GetPlayerIndex game player = playerIndex then
            // TODO: Unit test the fact that the player state is reset
            let gameWithResetPlayerState =
                game
                |> GameUtils.UpdatePlayer player (fun p ->
                    { p with
                        Creatures = p.Creatures |> Array.map (fun c -> { c with HasAttacked = false }) })

            Ok
                { gameWithResetPlayerState with
                    State = PlayerTurn((playerIndex + 1) % game.Players.Length) }
        else
            Error "You cannot end the turn when it is not your turn"
    | _ -> Error $"Unable to end turn in game state %s{game.State.GetType().Name}"

let addPlayer (game: Game) (playerGuid: Guid) (playerName: string) : Result<Game, string> =
    if game.State <> PlayerRegistration then
        Error $"Unable to add player in game state %s{game.State.GetType().Name}"
    else if game.Players |> Array.exists (fun p -> p.Id = playerGuid) then
        Error "You are already in this game"
    else
        let player =
            { Id = playerGuid
              Name = playerName
              Ready = false
              Class = None
              Creatures = [||] }

        let joinedArray = Array.append game.Players [| player |]

        let newPlayerArray =
            GameUtils.DeterministicShuffle [| player.Id; game.Id |] joinedArray

        Ok { game with Players = newPlayerArray }

let removePlayer (game: Game) (playerGuid: Guid) : Result<Game, string> =
    // TODO: Test leaving an ended game
    // TODO: Test leaving a game giving the other player the win
    match game.State with
    | Ended _ -> Error "The game has already ended"
    | _ ->
        if game.Players |> Array.exists (fun p -> p.Id = playerGuid) then
            let newPlayerArray = game.Players |> Array.filter (fun p -> p.Id <> playerGuid)

            Ok
                { game with
                    Players = newPlayerArray
                    State =
                        match game.State with
                        | PlayerTurn _ when newPlayerArray.Length = 0 -> Ended(None)
                        | PlayerTurn _ when newPlayerArray.Length = 1 -> Ended(Some newPlayerArray[0].Id)
                        | PlayerTurn playerIndex -> PlayerTurn(playerIndex % newPlayerArray.Length)
                        | _ -> game.State }
        else
            Error "You are not in this game"

let ready (game: Game) (playerGuid: Guid) : Result<Game, string> =
    let player = GameUtils.GetPlayerById game playerGuid

    if game.State <> PlayerRegistration then
        Error $"Unable to ready player in game state %s{game.State.GetType().Name}"
    else if Option.isNone player then
        Error "You are not in this game"
    else if player.Value.Class.IsNone then
        Error "You must select a class before you can ready"
    else
        let updatedGame =
            game
            |> GameUtils.UpdatePlayer player.Value.Id (fun p -> { p with Ready = true })

        if
            updatedGame.Players |> Array.forall (fun p -> p.Ready)
            && updatedGame.Players.Length > 1
        then
            Ok
                { updatedGame with
                    State = PlayerTurn 0 }
        else
            Ok updatedGame

let selectClass (game: Game) (player: Guid) (newClass: Class) : Result<Game, String> =
    if game.State <> PlayerRegistration then
        Error $"Unable to select class in game state %s{game.State.GetType().Name}"
    else
        let updatedGame =
            game
            |> GameUtils.UpdatePlayer player (fun p ->
                { p with
                    Class = Some newClass
                    Creatures = Creatures.ClassCreatures[newClass] })

        Ok updatedGame

// TODO: Unit test
let attack
    (game: Game)
    (player: Guid)
    (attackingCreatureIndex: int)
    (targetPlayer: Guid)
    (targetCreatureIndex: int)
    : Result<Game, String> =
    let attackingPlayerOpt = GameUtils.GetPlayerById game player
    let targetPlayerOpt = GameUtils.GetPlayerById game targetPlayer

    match game.State with
    | PlayerTurn playerIndex ->
        if attackingPlayerOpt.IsNone then
            Error "You are not in this game"
        else if targetPlayerOpt.IsNone then
            Error "The target player is not in this game"
        else if GameUtils.GetPlayerIndex game player <> playerIndex then
            Error "You cannot attack when it is not your turn"
        else
            let attackingPlayer = attackingPlayerOpt.Value
            let targetPlayer = targetPlayerOpt.Value

            let attackingCreatureOpt =
                attackingPlayer.Creatures |> Array.tryItem attackingCreatureIndex

            let targetCreatureOpt = targetPlayer.Creatures |> Array.tryItem targetCreatureIndex

            if attackingCreatureOpt.IsNone then
                Error "The attacking creature does not exist"
            else if targetCreatureOpt.IsNone then
                Error "The target creature does not exist"
            else if attackingCreatureOpt.Value.HasAttacked then
                Error "The attacking creature has already attacked"
            else
                let attackingCreature = attackingCreatureOpt.Value
                let targetCreature = targetCreatureOpt.Value

                let updatedAttacker =
                    { attackingCreature with
                        Health = attackingCreature.Health - targetCreature.Attack
                        HasAttacked = true }

                let updatedTarget =
                    { targetCreature with
                        Health = targetCreature.Health - attackingCreature.Attack }

                game
                |> GameUtils.UpdatePlayer player (fun p ->
                    { p with
                        Creatures =
                            p.Creatures
                            |> Array.updateAt attackingCreatureIndex updatedAttacker
                            |> Array.filter (fun c -> c.Health > 0) })
                |> GameUtils.UpdatePlayer targetPlayer.Id (fun p ->
                    { p with
                        Creatures =
                            p.Creatures
                            |> Array.updateAt targetCreatureIndex updatedTarget
                            |> Array.filter (fun c -> c.Health > 0) })
                |> checkWinState
                |> Ok
    | _ -> Error $"Unable to attack in game state %s{game.State.GetType().Name}"


let Apply (game: Game) (action: Action) : Result<Game, string> =
    match action with
    | EndTurn player -> endTurn game player
    | AddPlayer(player, playerName) -> addPlayer game player playerName
    | RemovePlayer player -> removePlayer game player
    | Ready player -> ready game player
    | SelectClass(player, newClass) -> selectClass game player newClass
    | Attack(player, attackingCreatureIndex, targetPlayer, targetCreatureIndex) ->
        attack game player attackingCreatureIndex targetPlayer targetCreatureIndex

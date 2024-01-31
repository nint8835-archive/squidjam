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

let rec drawMutation (player: Player) : Player =
    if player.MutationDeck.Length = 0 then
        drawMutation
            { player with
                // TODO: Shuffle deck
                MutationDeck = Mutations.ClassMutations[player.Class.Value] }
    else
        let mutation = Array.head player.MutationDeck
        let newMutationDeck = player.MutationDeck |> Array.skip 1

        { player with
            MutationHand = Array.append [| mutation |] player.MutationHand
            MutationDeck = newMutationDeck }

type Action =
    | EndTurn of Player: Guid
    | AddPlayer of Player: Guid * Name: string
    | RemovePlayer of Player: Guid
    | Ready of Player: Guid
    | SelectClass of Player: Guid * Class: Class
    | Attack of Player: Guid * AttackingCreatureIndex: int * TargetPlayer: Guid * TargetCreatureIndex: int
    | Mutate of Player: Guid * MutationIndex: int * TargetPlayer: Guid * TargetCreatureIndex: int

let endTurn (game: Game) (player: Guid) : Result<Game, string> =
    match game.State with
    | PlayerTurn playerIndex ->
        if GameUtils.GetPlayerIndex game player = playerIndex then
            // TODO: Unit test the fact that the player state is reset
            let gameWithResetPlayerState =
                game
                |> GameUtils.UpdatePlayer player (fun p ->
                    { p with
                        RemainingEnergy = Math.Min(p.MaxEnergy + 1, 10)
                        MaxEnergy = Math.Min(p.MaxEnergy + 1, 10)
                        Creatures = p.Creatures |> Array.map (fun c -> { c with HasAttacked = false }) }
                    |> drawMutation)

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
              Creatures = [||]
              RemainingEnergy = 1
              MaxEnergy = 1
              MutationDeck = [||]
              MutationHand = [||] }

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
            let updatedPlayers =
                updatedGame.Players
                // TODO: Shuffle mutations before drawing
                |> Array.map (fun p -> p |> drawMutation |> drawMutation |> drawMutation)

            Ok
                { updatedGame with
                    State = PlayerTurn 0
                    Players = updatedPlayers }
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
                    Creatures = Creatures.ClassCreatures[newClass]
                    MutationDeck = Mutations.ClassMutations[newClass] })

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

// TODO: Test
let mutate
    (game: Game)
    (playerGuid: Guid)
    (mutationIndex: int)
    (targetPlayerGuid: Guid)
    (targetCreatureIndex: int)
    : Result<Game, string> =
    let attackingPlayerOpt = GameUtils.GetPlayerById game playerGuid
    let targetPlayerOpt = GameUtils.GetPlayerById game targetPlayerGuid

    match game.State with
    | PlayerTurn playerIndex ->
        if attackingPlayerOpt.IsNone then
            Error "You are not in this game"
        else if targetPlayerOpt.IsNone then
            Error "The target player is not in this game"
        else if GameUtils.GetPlayerIndex game playerGuid <> playerIndex then
            Error "You cannot mutate when it is not your turn"
        else
            let attackingPlayer = attackingPlayerOpt.Value
            let targetPlayer = targetPlayerOpt.Value

            let mutationOpt = attackingPlayer.MutationHand |> Array.tryItem mutationIndex
            let targetCreatureOpt = targetPlayer.Creatures |> Array.tryItem targetCreatureIndex

            // TODO: Resource requirement
            if targetCreatureOpt.IsNone then
                Error "The target creature does not exist"
            else if mutationOpt.IsNone then
                Error "The mutation does not exist"
            else if attackingPlayer.RemainingEnergy < mutationOpt.Value.EnergyCost then
                Error "You do not have enough energy to mutate"
            else
                let mutation = mutationOpt.Value
                let targetCreature = targetCreatureOpt.Value

                let targetMutationSlotIndex =
                    targetCreature.Mutations |> Array.tryFindIndex Option.isNone

                if targetMutationSlotIndex.IsNone then
                    Error "The target creature has no mutation slots left"
                else
                    let mutatedCreature = targetCreature |> Mutations.Mutators[mutation.Name]

                    let updatedTarget =
                        { mutatedCreature with
                            Mutations =
                                targetCreature.Mutations
                                |> Array.updateAt targetMutationSlotIndex.Value (Some mutation) }

                    game
                    |> GameUtils.UpdatePlayer targetPlayerGuid (fun p ->
                        { p with
                            Creatures =
                                p.Creatures
                                |> Array.updateAt targetCreatureIndex updatedTarget
                                |> Array.filter (fun c -> c.Health > 0) })
                    |> GameUtils.UpdatePlayer playerGuid (fun p ->
                        { p with
                            RemainingEnergy = p.RemainingEnergy - mutation.EnergyCost
                            MutationHand = p.MutationHand |> Array.removeAt mutationIndex })
                    |> checkWinState
                    |> Ok
    | _ -> Error $"Unable to mutate in game state %s{game.State.GetType().Name}"


let Apply (game: Game) (action: Action) : Result<Game, string> =
    match action with
    | EndTurn player -> endTurn game player
    | AddPlayer(player, playerName) -> addPlayer game player playerName
    | RemovePlayer player -> removePlayer game player
    | Ready player -> ready game player
    | SelectClass(player, newClass) -> selectClass game player newClass
    | Attack(player, attackingCreatureIndex, targetPlayer, targetCreatureIndex) ->
        attack game player attackingCreatureIndex targetPlayer targetCreatureIndex
    | Mutate(playerGuid, mutationIndex, targetPlayerGuid, targetCreatureIndex) ->
        mutate game playerGuid mutationIndex targetPlayerGuid targetCreatureIndex

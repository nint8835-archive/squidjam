module Squidjam.Game.Actions

open System

let ClassCreatures = Map<Class, Creature array>([
    (Class.Grack, [| {Health = 10}; {Health = 15}; {Health = 10} |])
    (Class.Gump, [| {Health = 5}; {Health = 5}; {Health = 5}; {Health = 5}; {Health = 5}; {Health = 5}; {Health = 5} |])
])

type Action =
    | EndTurn of Player: Guid
    | AddPlayer of Player: Guid * Name: string
    | RemovePlayer of Player: Guid
    | Ready of Player: Guid
    | SelectClass of Player: Guid * Class: Class

let endTurn (game: Game) (player: Guid) : Result<Game, string> =
    match game.State with
    | PlayerTurn playerIndex ->
        if GameUtils.GetPlayerIndex game player = playerIndex then
            Ok { game with State = PlayerTurn((playerIndex + 1) % game.Players.Length) }
        else
            Error "You cannot end the turn when it is not your turn"
    | _ -> Error $"Unable to end turn in game state %s{game.State.GetType().Name}"

let addPlayer (game: Game) (playerGuid: Guid) (playerName: string) : Result<Game, string> =
    if game.State <> PlayerRegistration then
        Error $"Unable to add player in game state %s{game.State.GetType().Name}"
    else if game.Players |> Array.exists (fun p -> p.Id = playerGuid) then
        Error "You are already in this game"
    else
        let player = {
            Id = playerGuid
            Name = playerName
            Ready = false
            Class = None
            Creatures = [||] 
        }

        let joinedArray = Array.append game.Players [| player |]

        let newPlayerArray = GameUtils.DeterministicShuffle [| player.Id; game.Id |] joinedArray

        Ok { game with Players = newPlayerArray }

let removePlayer (game: Game) (playerGuid: Guid) : Result<Game, string> =
    if game.Players |> Array.exists (fun p -> p.Id = playerGuid) then
        let newPlayerArray = game.Players |> Array.filter (fun p -> p.Id <> playerGuid)

        Ok { game with
                Players = newPlayerArray;
                State = match game.State with
                        | PlayerTurn _ when newPlayerArray.Length = 0 -> Ended(None)
                        | PlayerTurn playerIndex -> PlayerTurn(playerIndex % newPlayerArray.Length)
                        | _ -> game.State
             }
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
        let updatedGame = GameUtils.UpdatePlayer game player.Value.Id (fun p -> { p with Ready = true })
        
        if updatedGame.Players |> Array.forall (_.Ready) && updatedGame.Players.Length > 1 then
            Ok { updatedGame with State = PlayerTurn 0 }
        else
            Ok updatedGame

let selectClass (game: Game) (player: Guid) (newClass: Class) : Result<Game, String> =
    if game.State <> PlayerRegistration then
        Error $"Unable to select class in game state %s{game.State.GetType().Name}"
    else
        let updatedGame = GameUtils.UpdatePlayer game player (fun p -> { p with Class = Some newClass; Creatures = ClassCreatures[newClass] })

        Ok updatedGame


let Apply (game: Game) (action: Action) : Result<Game, string> =
    match action with
    | EndTurn player -> endTurn game player
    | AddPlayer (player, playerName) -> addPlayer game player playerName
    | RemovePlayer player -> removePlayer game player
    | Ready player -> ready game player
    | SelectClass (player, newClass) -> selectClass game player newClass
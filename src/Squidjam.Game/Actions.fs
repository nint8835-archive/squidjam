module Squidjam.Game.Actions

type Action = | EndTurn

let Apply (game: Game) (action: Action) : Result<Game, string> =
    match action with
    | EndTurn ->
        match game.state with
        | PlayerTurn playerIndex -> Ok { game with state = PlayerTurn((playerIndex + 1) % game.players.Length) }
        | _ -> Error $"Unable to end turn in game state %s{game.state.GetType().Name}"

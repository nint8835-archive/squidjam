namespace Squidjam.Game

open System

type GameState =
    | PlayerTurn of int
    | Ended of Guid option


type Player = { id: Guid }

type Game =
    { state: GameState
      players: Player array }

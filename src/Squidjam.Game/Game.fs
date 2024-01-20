namespace Squidjam.Game

open System

type GameState =
    | PlayerRegistration
    | PlayerTurn of int
    | Ended of Guid option


type Player = { id: Guid; ready: bool }

type Game =
    { id: Guid
      state: GameState
      players: Player array }

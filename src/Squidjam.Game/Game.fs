namespace Squidjam.Game

open System

type GameState =
    | PlayerRegistration
    | PlayerTurn of int
    | Ended of Guid option


type Player = { Id: Guid; Ready: bool }

type Game =
    { Id: Guid
      State: GameState
      Players: Player array }

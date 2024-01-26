namespace Squidjam.Game

open System

type Class =
    | Grack
    | Gump

type GameState =
    | PlayerRegistration
    | PlayerTurn of PlayerIndex: int
    | Ended of Winner: Guid option


type Player =
    { Id: Guid
      Ready: bool
      Class: Class option }

type Game =
    { Id: Guid
      State: GameState
      Players: Player array }

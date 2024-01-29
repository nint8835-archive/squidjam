namespace Squidjam.Game

open System

type Class =
    | Grack
    | Gump

type GameState =
    | PlayerRegistration
    | PlayerTurn of PlayerIndex: int
    | Ended of Winner: Guid option

type Creature =
    { Name: string
      Health: int
      // TODO: Should attack be caused by mutations, rather than a direct attribute?
      Attack: int
      HasAttacked: bool }


type Player =
    { Id: Guid
      Name: string
      Ready: bool
      Class: Class option
      Creatures: Creature array }

type Game =
    { Id: Guid
      State: GameState
      Players: Player array }

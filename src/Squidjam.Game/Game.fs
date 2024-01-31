namespace Squidjam.Game

open System

type Class =
    | Grack
    | Gump

type GameState =
    | PlayerRegistration
    | PlayerTurn of PlayerIndex: int
    | Ended of Winner: Guid option

type Mutation =
    { Name: string
      Description: string
      EnergyCost: int }

and Creature =
    { Name: string
      Health: int
      Attack: int
      HasAttacked: bool
      Mutations: Mutation option array }


type Player =
    { Id: Guid
      Name: string
      Ready: bool
      Class: Class option
      Creatures: Creature array

      RemainingEnergy: int
      MaxEnergy: int

      MutationDeck: Mutation array
      MutationHand: Mutation array }

type Game =
    { Id: Guid
      State: GameState
      Players: Player array }

type OnApplyMutatorArgs =
    { SourcePlayer: Guid
      TargetPlayer: Guid
      TargetCreatureIndex: int }

type MutatorFunc<'args> = 'args -> Game -> Game

type Mutator =
    { OnApply: MutatorFunc<OnApplyMutatorArgs> option }

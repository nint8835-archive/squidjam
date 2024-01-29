module Squidjam.Game.Creatures

let Rat =
    { Name = "Rat"
      Health = 5
      Attack = 1
      HasAttacked = false }

let ClassCreatures =
    Map<Class, Creature array>(
        [ (Class.Grack,
           [| { Name = "???"
                Health = 10
                Attack = 5
                HasAttacked = false }
              { Name = "???"
                Health = 15
                Attack = 3
                HasAttacked = false }
              { Name = "???"
                Health = 10
                Attack = 5
                HasAttacked = false } |])
          (Class.Gump, [| Rat; Rat; Rat; Rat; Rat; Rat; Rat |]) ]
    )

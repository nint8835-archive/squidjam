module Squidjam.Game.Creatures

let Rat =
    { Name = "Rat"
      Health = 5
      Attack = 1
      HasAttacked = false
      Mutations = Array.create 2 None }

let ClassCreatures =
    Map<Class, Creature array>(
        [ (Class.Grack,
           [| { Name = "???"
                Health = 10
                Attack = 5
                HasAttacked = false
                Mutations = Array.create 5 None }
              { Name = "???"
                Health = 15
                Attack = 3
                HasAttacked = false
                Mutations = Array.create 3 None }
              { Name = "???"
                Health = 10
                Attack = 5
                HasAttacked = false
                Mutations = Array.create 5 None } |])
          (Class.Gump, Array.create 7 Rat) ]
    )

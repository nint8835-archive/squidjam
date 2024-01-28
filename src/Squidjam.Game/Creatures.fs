module Squidjam.Game.Creatures

let Rat = { Name = "Rat"; Health = 5 }

let ClassCreatures =
    Map<Class, Creature array>(
        [ (Class.Grack,
           [| { Name = "???"; Health = 10 }
              { Name = "???"; Health = 15 }
              { Name = "???"; Health = 10 } |])
          (Class.Gump, [| Rat; Rat; Rat; Rat; Rat; Rat; Rat |]) ]
    )

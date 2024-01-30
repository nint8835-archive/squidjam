module Squidjam.Game.Mutations

let ClassMutations =
    Map<Class, Mutation array>([| (Class.Grack, [||]); (Class.Gump, [||]) |])

let Mutators = Map<string, Creature -> Creature> [||]

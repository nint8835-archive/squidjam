module Squidjam.Game.Mutations

[<Literal>]
let GrackStickerID = "Grack Sticker"

let GrackSticker =
    { Name = GrackStickerID
      Description = "A sticker of Grack. Grants 5 health." }

let ClassMutations =
    Map<Class, Mutation array>([| (Class.Grack, [| GrackSticker |]); (Class.Gump, [| GrackSticker |]) |])

let Mutators =
    Map<string, Creature -> Creature>
        [| (GrackStickerID,
            fun creature ->
                { creature with
                    Health = creature.Health + 5 }) |]

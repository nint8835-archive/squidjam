module Squidjam.Game.Mutations


let applyMutator (mutator: MutatorFunc<'a> option) (args: 'a) (game: Game) : Game =
    match mutator with
    | Some(mutatorFunc) -> mutatorFunc args game
    | None -> game

let simpleCreatureMutator (mutatorFunc: Creature -> Creature) (mutatorArgs: OnApplyMutatorArgs) (game: Game) : Game =
    let targetCreature =
        GameUtils.GetPlayerById game mutatorArgs.TargetPlayer
        |> Option.get
        |> (fun p -> p.Creatures)
        |> Array.item mutatorArgs.TargetCreatureIndex

    let mutatedCreature = targetCreature |> mutatorFunc

    game
    |> GameUtils.UpdatePlayer mutatorArgs.TargetPlayer (fun p ->
        { p with
            Creatures = p.Creatures |> Array.updateAt mutatorArgs.TargetCreatureIndex mutatedCreature })

[<Literal>]
let GrackStickerID = "Grack Sticker"

let GrackSticker =
    { Name = GrackStickerID
      Description = "A sticker of Grack. Grants 5 health."
      EnergyCost = 3 }

let ClassMutations =
    Map<Class, Mutation array>([| (Class.Grack, [| GrackSticker |]); (Class.Gump, [| GrackSticker |]) |])

let Mutators =
    Map<string, Mutator>
        [| (GrackStickerID,
            { OnApply = Some(simpleCreatureMutator (fun c -> { c with Health = c.Health + 5 }))
              OnAttacking = None
              OnDefending = None }) |]

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

[<Literal>]
let BorikID = "borik!"

let Borik =
    { Name = BorikID
      Description = "Content-aware scale the target creature. Doubles attack, but halves health."
      EnergyCost = 3 }

[<Literal>]
let HoloGrackID = "Holograck Sticker"

let HoloGrack =
    { Name = HoloGrackID
      Description =
        "A holographic sticker of Grack. Its shine distracts enemies, protecting a creature from a single attack."
      EnergyCost = 5 }

let ClassMutations =
    Map<Class, Mutation array>(
        [| (Class.Grack, [| GrackSticker; Borik; HoloGrack |])
           (Class.Gump, [| GrackSticker |]) |]
    )

let Mutators =
    Map<string, Mutator>
        [| (GrackStickerID,
            { OnApply = Some(simpleCreatureMutator (fun c -> { c with Health = c.Health + 5 }))
              OnAttacking = None
              OnDefending = None })

           (BorikID,
            { OnApply =
                Some(
                    simpleCreatureMutator (fun c ->
                        { c with
                            Health = c.Health / 2
                            Attack = c.Attack * 2 })
                )
              OnAttacking = None
              OnDefending = None })

           (HoloGrackID,
            { OnApply = None
              OnAttacking = None
              OnDefending =
                Some(fun args g ->
                    let defendingCreature =
                        GameUtils.GetPlayerById g args.DefendingPlayer
                        |> Option.get
                        |> (fun p -> p.Creatures)
                        |> Array.item args.DefendingCreatureIndex

                    let attackingCreature =
                        GameUtils.GetPlayerById g args.AttackingPlayer
                        |> Option.get
                        |> (fun p -> p.Creatures)
                        |> Array.item args.AttackingCreatureIndex

                    let newDefendingCreature =
                        { defendingCreature with
                            Health = defendingCreature.Health + attackingCreature.Attack
                            Mutations =
                                defendingCreature.Mutations
                                |> Array.filter (fun m -> m.IsNone || m.Value.Name <> HoloGrackID) }

                    g
                    |> GameUtils.UpdatePlayer args.DefendingPlayer (fun p ->
                        { p with
                            Creatures = p.Creatures |> Array.updateAt args.DefendingCreatureIndex newDefendingCreature })) }) |]

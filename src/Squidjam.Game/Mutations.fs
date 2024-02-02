module Squidjam.Game.Mutations

open System

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
      Description = "A sticker of Grack. Restores 1 health."
      EnergyCost = 2 }

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

[<Literal>]
let RatKingID = "Rat King"

let RatKing =
    { Name = RatKingID
      Description = "Merges three creatures into one. Does not consume a mutation slot."
      EnergyCost = 5 }

[<Literal>]
let ManchuWokID = "Manchu Wok"

let ManchuWok =
    { Name = ManchuWokID
      Description = "A delicious, high quality meal from the UC. Restores 1 health."
      EnergyCost = 2 }

let ClassMutations =
    Map<Class, Mutation array>(
        [| (Class.Grack, [| GrackSticker; Borik; HoloGrack |])
           (Class.Gump, [| ManchuWok; RatKing |]) |]
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
                            Creatures = p.Creatures |> Array.updateAt args.DefendingCreatureIndex newDefendingCreature })) })
           (RatKingID,
            { OnApply =
                Some(fun args g ->
                    let targetPlayerCreatures =
                        GameUtils.GetPlayerById g args.TargetPlayer
                        |> Option.get
                        |> (fun p -> p.Creatures)

                    let startIndex = Math.Max(0, args.TargetCreatureIndex - 1)

                    let count =
                        Math.Min(targetPlayerCreatures.Length - 1, args.TargetCreatureIndex + 1)
                        - startIndex
                        + 1

                    let targetCreatures = targetPlayerCreatures |> Array.sub <|| (startIndex, count)

                    let mergedTarget =
                        targetCreatures
                        |> Array.reduce (fun t c ->
                            { t with
                                Attack = t.Attack + c.Attack
                                Health = t.Health + c.Health
                                Mutations = Array.append t.Mutations c.Mutations })

                    let finalizedMergedTarget =
                        { mergedTarget with
                            Name = "Rat King"
                            Mutations =
                                mergedTarget.Mutations
                                |> Array.map (fun m ->
                                    match m with
                                    | None -> None
                                    | Some(mutVal) when mutVal.Name = RatKingID -> None
                                    | Some(value) -> Some(value)) }

                    let newCreatures =
                        targetPlayerCreatures
                        |> Array.removeManyAt startIndex count
                        |> Array.insertAt startIndex finalizedMergedTarget

                    g
                    |> GameUtils.UpdatePlayer args.TargetPlayer (fun p -> { p with Creatures = newCreatures }))
              OnAttacking = None
              OnDefending = None })

           (ManchuWokID,
            { OnApply = Some(simpleCreatureMutator (fun c -> { c with Health = c.Health + 5 }))
              OnAttacking = None
              OnDefending = None }) |]

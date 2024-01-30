import { toast } from 'sonner';
import { usePerformAction } from '../queries/api/squidjamComponents';
import * as Schema from '../queries/api/squidjamSchemas';
import { useStore } from '../store';
import { cn } from '../util';

const classClasses: Record<Exclude<Schema.Player['class'], null>['type'], string> = {
    // TODO: Different font for Grack?
    Grack: 'heropattern-floatingcogs-zinc-950 font-mono',
    Gump: 'heropattern-eyes-zinc-950 font-gump',
};

export default function Creature({
    creature,
    creatureIndex,
    player,
}: {
    creature: Schema.Creature;
    creatureIndex: number;
    player: Schema.Player;
}) {
    const {
        playerId: currentPlayer,
        setAttackingCreatureIndex,
        attackingCreatureIndex,
        selectedMutationIndex,
        setSelectedMutationIndex,
        currentGame: { id: gameId, state, players },
    } = useStore();
    const { mutateAsync: performAction } = usePerformAction({ onError: (err) => toast.error(err.stack) });

    const isCurrentPlayersCreature = player.id === currentPlayer;
    const isCurrentPlayersTurn =
        state.type === 'PlayerTurn' && state.playerIndex === players.findIndex((p) => p.id === currentPlayer);
    const isMutatable = creature.mutations.filter((m) => m === null).length > 0;

    return (
        <div
            className={cn(
                'flex w-48 min-w-48 flex-col items-center rounded-md border-[1px] border-black bg-black bg-opacity-30 transition-all',
                classClasses[player.class!.type],
                isCurrentPlayersTurn &&
                    ((isCurrentPlayersCreature && !creature.hasAttacked) || attackingCreatureIndex !== undefined) &&
                    'cursor-pointer hover:border-emerald-500',
                isCurrentPlayersCreature && attackingCreatureIndex === creatureIndex && 'border-emerald-600',
                creature.hasAttacked && 'opacity-50',
                selectedMutationIndex !== undefined && isMutatable && 'cursor-pointer hover:border-pink-500',
            )}
            onClick={async () => {
                if (!isCurrentPlayersTurn) return;

                if (selectedMutationIndex !== undefined) {
                    await performAction({
                        pathParams: { gameId },
                        body: {
                            type: 'Mutate',
                            player: currentPlayer,
                            targetPlayer: player.id,
                            targetCreatureIndex: creatureIndex,
                            mutationIndex: selectedMutationIndex,
                        },
                    });
                    setSelectedMutationIndex(undefined);
                    return;
                }

                if (isCurrentPlayersCreature && attackingCreatureIndex === undefined) {
                    setAttackingCreatureIndex(creatureIndex);
                } else if (isCurrentPlayersCreature && attackingCreatureIndex === creatureIndex) {
                    setAttackingCreatureIndex(undefined);
                } else if (attackingCreatureIndex !== undefined) {
                    await performAction({
                        pathParams: {
                            gameId,
                        },
                        body: {
                            type: 'Attack',
                            player: currentPlayer,
                            attackingCreatureIndex,
                            targetPlayer: player.id,
                            targetCreatureIndex: creatureIndex,
                        },
                    });
                    setAttackingCreatureIndex(undefined);
                }
            }}
        >
            <div className="flex w-full justify-between border-b-[1px] border-black bg-black bg-opacity-35 px-2">
                <div className="flex w-6 flex-row">{creature.health}</div>
                <div className="flex flex-1 justify-center">{creature.name}</div>
                <div className="flex w-6 flex-row-reverse">{creature.attack}</div>
            </div>
            <div>
                {creature.mutations.map((mutation, mutationIndex) => (
                    <div key={mutationIndex} className="flex justify-center">
                        {mutation ? <div>{mutation.name}</div> : <div className="italic opacity-75">Empty</div>}
                    </div>
                ))}
            </div>
        </div>
    );
}

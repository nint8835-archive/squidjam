import { CheckCircleIcon, XCircleIcon } from '@heroicons/react/20/solid';
import { toast } from 'sonner';
import { usePerformAction } from '../queries/api/squidjamComponents';
import * as Schema from '../queries/api/squidjamSchemas';
import { useStore } from '../store';
import { cn } from '../util';
import Creature from './Creature';

const playerColours = ['bg-red-900', 'bg-purple-900', 'bg-blue-900'];

const classes: Schema.SelectClass['class']['type'][] = ['Grack', 'Gump'];
const classClasses: Record<Schema.SelectClass['class']['type'], string> = {
    Grack: 'font-mono',
    Gump: 'font-gump',
};

const activeClassClasses: Record<Schema.SelectClass['class']['type'], string> = {
    Grack: 'heropattern-floatingcogs-zinc-950',
    Gump: 'heropattern-eyes-zinc-950',
};

export default function Player({ player, playerIndex }: { player: Schema.Player; playerIndex: number }) {
    const {
        currentGame: { id: gameId, state: gameState, players },
        playerId: currentPlayer,
        selectedMutationIndex,
        setSelectedMutationIndex,
        setAttackingCreatureIndex,
    } = useStore();
    const { mutateAsync: performAction } = usePerformAction({ onError: (err) => toast.error(err.stack) });

    const isCurrentPlayer = player.id === currentPlayer;
    const isPlayersTurn = gameState.type === 'PlayerTurn' && gameState.playerIndex === players.indexOf(player);

    const playerColour = playerColours[playerIndex % 3];

    return (
        <div className={cn('bg-opacity-50 p-2', playerColour, isPlayersTurn && 'border-l-2 border-l-white')}>
            <div className="flex flex-row items-center justify-between pb-2">
                <div className="flex-1 text-xl">
                    {player.name}{' '}
                    <span className="text-sm">
                        {'('}
                        {player.class?.type ?? 'No Class'}
                        {')'}
                    </span>
                </div>

                {/* Player stats */}
                {gameState.type === 'PlayerTurn' && (
                    <div className="flex flex-1 flex-row justify-center">
                        <div className="flex flex-row items-center gap-2 text-xl">
                            {player.creatures.map((c) => c.health).reduce((partialSum, a) => partialSum + a, 0)}
                            <span className="text-sm">HP</span>
                            {player.mutationHand.length}
                            <span className="text-sm">Mutations</span>
                            {`${player.remainingEnergy}/${player.maxEnergy}`}
                            <span className="text-sm">Energy</span>
                        </div>
                    </div>
                )}

                <div className="flex flex-1 flex-row-reverse">
                    {/* Ready status */}
                    {gameState.type === 'PlayerRegistration' && (
                        <div
                            className={cn(
                                'flex flex-row items-center gap-2 italic ',
                                player.ready ? 'text-green-500' : 'text-white text-opacity-25',
                            )}
                        >
                            {player.ready ? (
                                <>
                                    Ready <CheckCircleIcon className="w-6" />
                                </>
                            ) : (
                                <>
                                    Not Ready <XCircleIcon className="w-6" />
                                </>
                            )}
                        </div>
                    )}

                    {/* End turn button */}
                    {isPlayersTurn && isCurrentPlayer && (
                        <div>
                            <button
                                className="flex-1 rounded-sm bg-blue-600 px-2 transition-all hover:bg-blue-700"
                                onClick={async () => {
                                    await performAction({
                                        pathParams: { gameId },
                                        body: {
                                            type: 'EndTurn',
                                            player: player.id,
                                        },
                                    });
                                }}
                            >
                                End Turn
                            </button>
                        </div>
                    )}
                </div>
            </div>

            {/* Creature UI */}
            <div className="flex w-full flex-row justify-between gap-4 overflow-auto">
                {player.creatures.map((creature, creatureIndex) => (
                    <Creature creature={creature} creatureIndex={creatureIndex} player={player} />
                ))}
            </div>

            {/* Mutation UI */}
            {isCurrentPlayer && player.mutationHand.length > 0 && (
                <div className="flex w-full flex-row justify-between gap-4 overflow-auto pt-2">
                    {player.mutationHand.map((mutation, mutationIndex) => (
                        <div
                            key={mutationIndex}
                            className={cn(
                                'rounded-md border-[1px] bg-black bg-opacity-25 p-2 transition-all',
                                isPlayersTurn &&
                                    mutation.energyCost <= player.remainingEnergy &&
                                    'cursor-pointer hover:border-pink-500',
                                selectedMutationIndex === mutationIndex && 'border-pink-600',
                            )}
                            onClick={() => {
                                if (!isPlayersTurn) return;
                                if (mutation.energyCost > player.remainingEnergy) return;

                                setAttackingCreatureIndex(undefined);

                                if (selectedMutationIndex === mutationIndex) {
                                    setSelectedMutationIndex(undefined);
                                } else {
                                    setSelectedMutationIndex(mutationIndex);
                                }
                            }}
                        >
                            <div>{mutation.name}</div>
                            <div>{mutation.description}</div>
                        </div>
                    ))}
                </div>
            )}

            {/* Class selection / ready-up UI */}
            {gameState.type === 'PlayerRegistration' && isCurrentPlayer && (
                <div className="flex flex-col items-center justify-center gap-4">
                    <h1 className="text-xl">Select a class</h1>
                    <div className="space-x-4">
                        {classes.map((className) => (
                            <button
                                className={cn(
                                    'h-24 w-24 rounded-lg border-2 border-hidden border-black bg-opacity-5 text-lg transition-all hover:bg-black hover:bg-opacity-25',
                                    classClasses[className],
                                    player.class && player.class.type === className && 'border-solid',
                                    player.class && player.class.type === className && activeClassClasses[className],
                                )}
                                key={className}
                                onClick={async () => {
                                    await performAction({
                                        pathParams: { gameId },
                                        body: {
                                            type: 'SelectClass',
                                            class: { type: className },
                                            player: player.id,
                                        },
                                    });
                                }}
                            >
                                {className}
                            </button>
                        ))}
                    </div>
                    <button
                        className={cn(
                            'w-full rounded-lg p-2 text-lg transition-all',
                            player.class === null
                                ? 'cursor-not-allowed bg-red-800 bg-opacity-50 text-opacity-50'
                                : 'cursor-pointer bg-green-600 hover:bg-green-700',
                        )}
                        disabled={player.class === null}
                        title={player.class === null ? 'Select a class to continue' : 'Ready to play'}
                        onClick={async () => {
                            await performAction({
                                pathParams: { gameId },
                                body: {
                                    type: 'Ready',
                                    player: player.id,
                                },
                            });
                        }}
                    >
                        Ready
                    </button>
                </div>
            )}
        </div>
    );
}

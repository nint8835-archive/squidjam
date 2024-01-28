import { CheckCircleIcon, XCircleIcon } from '@heroicons/react/20/solid';
import { toast } from 'sonner';
import { usePerformAction } from '../queries/api/squidjamComponents';
import * as Schema from '../queries/api/squidjamSchemas';
import { useStore } from '../store';
import { cn } from '../util';

const playerColours = ['bg-red-900', 'bg-purple-900', 'bg-blue-900'];

const classes: Schema.SelectClass['class']['type'][] = ['Grack', 'Gump'];

export default function Player({ player, playerIndex }: { player: Schema.Player; playerIndex: number }) {
    const {
        currentGame: { id: gameId, state: gameState, players },
        playerId: currentPlayer,
    } = useStore();
    const { mutateAsync: performAction } = usePerformAction({ onError: (err) => toast.error(err.stack) });

    const isCurrentPlayer = player.id === currentPlayer;

    const playerColour = playerColours[playerIndex % 3];

    return (
        <div
            className={cn(
                'bg-opacity-50 p-2',
                playerColour,
                gameState.type === 'PlayerTurn' &&
                    gameState.playerIndex === players.indexOf(player) &&
                    'border-l-2 border-l-white',
            )}
        >
            <div className="flex flex-row justify-between">
                <div className="text-xl">
                    {player.name}{' '}
                    <span className="text-sm">
                        {'('}
                        {player.class?.type ?? 'No Class'}
                        {')'}
                    </span>
                </div>
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
            </div>

            {gameState.type === 'PlayerRegistration' && isCurrentPlayer && (
                <div className="flex flex-col items-center justify-center gap-4">
                    <h1 className="text-xl">Select a class</h1>
                    <div className="space-x-4">
                        {classes.map((className) => (
                            <button
                                className={cn(
                                    'h-24 w-24 rounded-lg border-2 border-hidden bg-opacity-5 text-lg transition-all hover:bg-black hover:bg-opacity-25',
                                    player.class && player.class.type === className && 'border-solid border-white',
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

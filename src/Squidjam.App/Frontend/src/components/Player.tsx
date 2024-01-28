import { toast } from 'sonner';
import { usePerformAction } from '../queries/api/squidjamComponents';
import * as Schema from '../queries/api/squidjamSchemas';
import { useStore } from '../store';
import { cn } from '../util';

const playerColours = ['bg-red-900', 'bg-purple-900', 'bg-blue-900'];

export default function Player({ player, playerIndex }: { player: Schema.Player; playerIndex: number }) {
    const {
        currentGame: { id: gameId },
        player: currentPlayer,
    } = useStore();
    const { mutateAsync: performAction } = usePerformAction({ onError: (err) => toast.error(err.stack) });

    const isCurrentPlayer = player.id === currentPlayer;

    const playerColour = playerColours[playerIndex % 3];

    const actions: { label: string; body: Schema.SelectClass | Schema.Ready | Schema.EndTurn }[] = [
        { label: 'Grack', body: { type: 'SelectClass', class: { type: 'Grack' }, player: currentPlayer } },
        { label: 'Gump', body: { type: 'SelectClass', class: { type: 'Gump' }, player: currentPlayer } },
        { label: 'Ready', body: { type: 'Ready', player: currentPlayer } },
        { label: 'End Turn', body: { type: 'EndTurn', player: currentPlayer } },
    ];

    return (
        <div className={cn('p-2', playerColour)}>
            <pre>{JSON.stringify(player, undefined, 2)}</pre>
            {isCurrentPlayer && (
                <div className="flex justify-between gap-4 px-16 pt-2">
                    {actions.map(({ label, body }) => (
                        <button
                            key={label}
                            className="flex-1 rounded-md bg-zinc-300 p-2 text-zinc-900 transition-all hover:bg-zinc-400"
                            onClick={async () => {
                                await performAction({
                                    pathParams: { gameId },
                                    body,
                                });
                            }}
                        >
                            {label}
                        </button>
                    ))}
                </div>
            )}
        </div>
    );
}

import { UserIcon } from '@heroicons/react/24/outline';
import { useCreateGame, useListGames, usePerformAction } from '../queries/api/squidjamComponents';
import * as Schema from '../queries/api/squidjamSchemas';
import { useStore } from '../store';
import { cn } from '../util';

const stateFormatters: Record<Schema.Game['state']['type'], (state: Schema.Game['state']) => string> = {
    PlayerRegistration: () => 'Waiting for players...',
    PlayerTurn: (state) => `Player ${(state as Schema.PlayerTurn).playerIndex + 1}'s turn`,
    Ended: (state) => {
        const { winner } = state as Schema.Ended;

        if (winner === null) {
            return 'Ended - Tie';
        }

        return `Ended - ${winner} won`;
    },
};

const stateClasses: Record<Schema.Game['state']['type'], string> = {
    PlayerRegistration: 'bg-green-800',
    PlayerTurn: 'bg-yellow-800',
    Ended: 'bg-red-800',
};

function StateBadge({ state }: { state: Schema.Game['state'] }) {
    return (
        <div className={cn('w-fit rounded-sm p-1 text-sm', stateClasses[state.type])}>
            {stateFormatters[state.type](state)}
        </div>
    );
}

export default function GameList() {
    const { data: games, isLoading, isError, refetch } = useListGames({}, { refetchInterval: 5000 });
    const { mutateAsync: createGame } = useCreateGame({});
    const { mutateAsync: performAction } = usePerformAction({});
    const { player } = useStore();

    return (
        <div className="space-y-4 p-4">
            <div className="flex flex-row justify-between">
                <h1 className="text-2xl">Games</h1>
                <button
                    className="rounded-md bg-emerald-600 px-4 py-2 text-white transition-all hover:bg-emerald-700"
                    onClick={async () => {
                        await createGame({});
                        refetch();
                    }}
                >
                    Create Game
                </button>
            </div>

            {isLoading && <div>Loading...</div>}
            {isError && <div>Error</div>}
            {games && (
                <div>
                    {Object.entries(games).map(([key, value]) => (
                        <div
                            key={key}
                            className="cursor-pointer rounded-md p-2 transition-all hover:bg-zinc-950"
                            onClick={async () => {
                                await performAction({
                                    pathParams: { gameId: key },
                                    body: { player, type: 'AddPlayer' },
                                });
                                refetch();
                            }}
                        >
                            <h2 className="text-xl">{key}</h2>
                            <div className="flex flex-row items-center justify-between">
                                <StateBadge state={value.state} />
                                <div className="flex">
                                    <UserIcon className="h-6 w-6" />
                                    {value.players.length}
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}

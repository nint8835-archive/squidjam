import { UserIcon } from '@heroicons/react/24/outline';
import { useNavigate } from 'react-router-dom';
import { useCreateGame, useListGames } from '../queries/api/squidjamComponents';
import * as Schema from '../queries/api/squidjamSchemas';
import { useStore } from '../store';
import { cn, stateFormatters } from '../util';

const stateClasses: Record<Schema.Game['state']['type'], string> = {
    PlayerRegistration: 'bg-green-800',
    PlayerTurn: 'bg-yellow-800',
    Ended: 'bg-red-800',
};

function StateBadge({ game }: { game: Schema.Game }) {
    return (
        <div className={cn('w-fit rounded-sm p-1 text-sm', stateClasses[game.state.type])}>
            {stateFormatters[game.state.type](game)}
        </div>
    );
}

export default function GameListPage() {
    const { playerName, setPlayerName } = useStore();

    const { data: games, isLoading, isError, refetch } = useListGames({}, { refetchInterval: 5000 });
    const { mutateAsync: createGame } = useCreateGame({});
    const navigate = useNavigate();

    return (
        <div className="space-y-4 p-4">
            <div className="flex flex-row justify-between">
                <h1 className="text-2xl">Games</h1>
                <div className="space-x-2">
                    <input
                        className="h-full rounded-md bg-zinc-900 p-2 text-right italic text-zinc-500 outline-none ring-zinc-500 transition-all focus:text-white focus:ring-2"
                        value={playerName}
                        size={playerName.length}
                        onChange={(e) => setPlayerName(e.target.value)}
                    ></input>
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
            </div>

            {isLoading && <div>Loading...</div>}
            {isError && <div>Error</div>}
            {games && (
                <div>
                    {Object.entries(games).map(([key, value]) => (
                        <div
                            key={key}
                            className="cursor-pointer rounded-md p-2 transition-all hover:bg-zinc-950"
                            onClick={() => {
                                navigate(`/game/${key}`);
                            }}
                        >
                            <h2 className="text-xl">{key}</h2>
                            <div className="flex flex-row items-center justify-between">
                                <StateBadge game={value} />
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

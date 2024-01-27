import { useQueryClient } from '@tanstack/react-query';
import { useCreateGame, useListGames, usePerformAction } from './queries/api/squidjamComponents';
import { queryKeyFn } from './queries/api/squidjamContext';
import { connection } from './signalr';
import { usePlayerStore } from './state/player';

export default function Test() {
    const { data: games, isLoading, error } = useListGames({}, { refetchInterval: 5000 });
    const { player } = usePlayerStore();

    const { mutateAsync: performAction, data: actionData, error: actionError } = usePerformAction({});
    const { mutateAsync: createGame, data: createGameData, error: createGameError } = useCreateGame({});

    const queryClient = useQueryClient();

    if (isLoading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>Error: {error.message}</div>;
    }

    if (games === undefined) {
        return <div>Games undefined</div>;
    }

    return (
        <div>
            {Object.entries(games).map(([key, value]) => (
                <pre key={key}>{JSON.stringify(value, undefined, 2)}</pre>
            ))}
            <pre>{JSON.stringify(player, undefined, 2)}</pre>
            <button
                onClick={async () => {
                    await performAction({
                        body: {
                            player,
                            type: 'AddPlayer',
                        },
                        pathParams: {
                            gameId: Object.keys(games)[0],
                        },
                    });
                    await queryClient.invalidateQueries({
                        queryKey: queryKeyFn({ operationId: 'listGames', path: '/api/games', variables: {} }),
                    });
                }}
            >
                DO THE THING
            </button>
            <pre>{JSON.stringify(actionData, undefined, 2)}</pre>
            <pre>{JSON.stringify(actionError, undefined, 2)}</pre>
            <button
                onClick={() => {
                    connection.send('example');
                }}
            >
                DO THE OTHER THING
            </button>
            <button
                onClick={async () => {
                    await createGame({});
                    await queryClient.invalidateQueries({
                        queryKey: queryKeyFn({ operationId: 'listGames', path: '/api/games', variables: {} }),
                    });
                }}
            >
                A THIRD, OTHER, DIFFERENT THING
            </button>
            <pre>{JSON.stringify(createGameData, undefined, 2)}</pre>
            <pre>{JSON.stringify(createGameError, undefined, 2)}</pre>
        </div>
    );
}

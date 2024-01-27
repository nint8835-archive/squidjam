import { useListGames } from './queries/api/squidjamComponents';
import { usePlayerStore } from './state/player';

export default function Test() {
    const { data: games, isLoading, error } = useListGames({});
    const { player } = usePlayerStore();

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
        </div>
    );
}

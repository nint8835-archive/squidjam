import { useEffect } from 'react';
import { v4 as uuidv4 } from 'uuid';
import { useListGames } from './queries/api/squidjamComponents';
import { usePlayerStore } from './state/player';

export default function Test() {
    const { data: games, isLoading, error } = useListGames({});
    const { player, setPlayer } = usePlayerStore();

    useEffect(() => {
        if (player !== undefined) {
            return;
        }

        setPlayer({
            id: uuidv4(),
            ready: false,
            class: null,
        });
    }, [player, setPlayer]);

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

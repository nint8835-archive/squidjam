import { useListGames } from './queries/api/squidjamComponents';

export default function Test() {
    const { data: games, isLoading, error } = useListGames({});

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
                <pre key={key}>{JSON.stringify(value)}</pre>
            ))}
        </div>
    );
}

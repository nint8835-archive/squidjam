import { useStore } from '../store';

export default function GamePage() {
    const { currentGame } = useStore();

    return <pre>{JSON.stringify(currentGame, undefined, 2)}</pre>;
}

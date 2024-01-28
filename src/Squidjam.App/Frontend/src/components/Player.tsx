import * as Schema from '../queries/api/squidjamSchemas';
import { useStore } from '../store';
import { cn } from '../util';

const playerColours = ['bg-red-900', 'bg-purple-900', 'bg-blue-900'];

export default function Player({ player, playerIndex }: { player: Schema.Player; playerIndex: number }) {
    const { player: currentPlayer } = useStore();
    const isCurrentPlayer = player.id === currentPlayer;

    const playerColour = playerColours[playerIndex % 3];

    return (
        <div className={cn('p-2', playerColour)}>
            <pre>{JSON.stringify(player, undefined, 2)}</pre>
        </div>
    );
}

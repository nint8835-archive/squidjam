import Player from '../components/Player';
import { useStore } from '../store';

export default function GamePage() {
    const { currentGame, player: currentPlayer } = useStore();

    return (
        <div className="flex h-full flex-col">
            <div>{JSON.stringify(currentGame.state)}</div>
            <div className="flex-1">
                {currentGame?.players.map((player, playerIndex) =>
                    player.id === currentPlayer ? null : (
                        <Player player={player} playerIndex={playerIndex} key={player.id} />
                    ),
                )}
            </div>

            <Player
                player={currentGame!.players.find((player) => player.id === currentPlayer)!}
                playerIndex={currentGame!.players.findIndex((player) => player.id === currentPlayer)!}
            />
        </div>
    );
}

import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';
import * as Schema from './queries/api/squidjamSchemas';

export function cn(...inputs: ClassValue[]) {
    return twMerge(clsx(inputs));
}

export const stateFormatters: Record<Schema.Game['state']['type'], (game: Schema.Game) => string> = {
    PlayerRegistration: () => 'Waiting for players...',
    PlayerTurn: (game) => `${game.players[(game.state as Schema.PlayerTurn).playerIndex].name}'s turn`,
    Ended: (game) => {
        const { winner } = game.state as Schema.Ended;

        if (winner === null) {
            return 'Ended - Tie';
        }

        return `Ended - ${game.players.find((player) => player.id === winner)?.name} won`;
    },
};

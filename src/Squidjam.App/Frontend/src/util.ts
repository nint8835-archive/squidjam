import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';
import * as Schema from './queries/api/squidjamSchemas';

export function cn(...inputs: ClassValue[]) {
    return twMerge(clsx(inputs));
}

export const stateFormatters: Record<Schema.Game['state']['type'], (state: Schema.Game['state']) => string> = {
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

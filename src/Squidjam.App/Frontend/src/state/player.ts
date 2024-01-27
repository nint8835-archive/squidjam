import { create } from 'zustand';
import { devtools, persist } from 'zustand/middleware';
import * as Schema from '../queries/api/squidjamSchemas';

export interface PlayerState {
    player?: Schema.Player;
    setPlayer: (player: Schema.Player | undefined) => void;
}

export const usePlayerStore = create<PlayerState>()(
    devtools(
        persist(
            (set) => ({
                setPlayer: (player) => set((state) => ({ ...state, player: player }), undefined, 'setPlayer'),
            }),
            {
                name: 'player-state',
            },
        ),
    ),
);

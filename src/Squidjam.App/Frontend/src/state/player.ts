import { v4 as uuid4 } from 'uuid';
import { create } from 'zustand';
import { devtools, persist } from 'zustand/middleware';

export interface PlayerState {
    player: string;
}

export const usePlayerStore = create<PlayerState>()(
    devtools(
        persist(() => ({ player: uuid4() }), {
            name: 'player-state',
        }),
    ),
);

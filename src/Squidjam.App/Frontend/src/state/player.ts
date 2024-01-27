import { create } from 'zustand';
import { devtools, persist } from 'zustand/middleware';

export interface PlayerState {}

export const usePlayerStore = create<PlayerState>()(devtools(persist(() => ({}), { name: 'player-state' })));

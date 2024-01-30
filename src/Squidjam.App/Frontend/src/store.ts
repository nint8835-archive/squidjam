import * as signalR from '@microsoft/signalr';
import { v4 as uuid4 } from 'uuid';
import { create } from 'zustand';
import { devtools, persist } from 'zustand/middleware';
import * as Schema from './queries/api/squidjamSchemas';

export interface Store {
    playerId: string;

    playerName: string;
    setPlayerName: (name: string) => void;

    attackingCreatureIndex?: number;
    setAttackingCreatureIndex: (index?: number) => void;

    selectedMutationIndex?: number;
    setSelectedMutationIndex: (index?: number) => void;

    currentGame: Schema.Game;
    signalRConnection?: signalR.HubConnection;
    signalRState: signalR.HubConnectionState;
    signalRConnectionError?: string;
    setupSignalR: () => void;
}

export const useStore = create<Store>()(
    devtools(
        persist(
            (set, get) => ({
                playerId: uuid4(),

                playerName: `Unnamed Player ${Math.floor(Math.random() * 1000)}`,
                setPlayerName: (name) => set({ playerName: name }, undefined, 'setPlayerName'),

                setAttackingCreatureIndex: (index) =>
                    set({ attackingCreatureIndex: index }, undefined, 'setAttackingCreatureIndex'),

                setSelectedMutationIndex: (index) =>
                    set({ selectedMutationIndex: index }, undefined, 'setSelectedMutationIndex'),

                signalRState: signalR.HubConnectionState.Disconnected,
                currentGame: { id: '', players: [], state: { type: 'PlayerRegistration' } },
                setupSignalR: async () => {
                    if (get().signalRConnection !== undefined) {
                        throw new Error('SignalR connection already set up');
                    }

                    const connection = new signalR.HubConnectionBuilder()
                        .withUrl(`/api/realtime?playerId=${get().playerId}`)
                        .withAutomaticReconnect()
                        .build();
                    set(
                        { signalRConnection: connection, signalRState: signalR.HubConnectionState.Connecting },
                        undefined,
                        'signalRSetup',
                    );

                    connection.on('GameUpdated', (game: Schema.Game) => {
                        set(
                            { currentGame: game, attackingCreatureIndex: undefined, selectedMutationIndex: undefined },
                            undefined,
                            'updateGameFromSignalR',
                        );
                    });

                    function updateSignalRConnectionState() {
                        set({ signalRState: connection.state }, undefined, 'signalRStateChanged');
                    }

                    connection.onreconnected(updateSignalRConnectionState);
                    connection.onreconnecting(updateSignalRConnectionState);
                    connection.onclose(updateSignalRConnectionState);

                    try {
                        await connection.start();
                        updateSignalRConnectionState();
                    } catch (e) {
                        const errorMessage = e instanceof Error ? e.message : JSON.stringify(e);
                        set(
                            { signalRConnectionError: errorMessage, signalRState: connection.state },
                            undefined,
                            'signalRConnectionError',
                        );
                        updateSignalRConnectionState();
                    }
                },
            }),
            {
                name: 'squidjam-state',
                partialize: (state) => ({ playerId: state.playerId, playerName: state.playerName }),
            },
        ),
    ),
);

useStore.getState().setupSignalR();

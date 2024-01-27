import * as signalR from '@microsoft/signalr';
import { v4 as uuid4 } from 'uuid';
import { create } from 'zustand';
import { devtools, persist } from 'zustand/middleware';
import * as Schema from './queries/api/squidjamSchemas';

export interface Store {
    player: string;

    currentGame?: Schema.Game;
    signalRConnection?: signalR.HubConnection;
    signalRState: signalR.HubConnectionState;
    setupSignalR: () => void;
}

export const useStore = create<Store>()(
    devtools(
        persist(
            (set, get) => ({
                player: uuid4(),
                signalRState: signalR.HubConnectionState.Disconnected,
                setupSignalR: async () => {
                    if (get().signalRConnection) {
                        throw new Error('SignalR connection already set up');
                    }

                    const connection = new signalR.HubConnectionBuilder()
                        .withUrl(`/api/realtime?playerId=${get().player}`)
                        .withAutomaticReconnect()
                        .build();
                    set(
                        { signalRConnection: connection, signalRState: signalR.HubConnectionState.Connecting },
                        undefined,
                        'signalRSetup',
                    );

                    connection.on('GameUpdated', (game: Schema.Game) => {
                        set({ currentGame: game }, undefined, 'updateGameFromSignalR');
                    });

                    function updateSignalRConnectionState() {
                        set({ signalRState: connection.state }, undefined, 'signalRStateChanged');
                    }

                    connection.onreconnected(updateSignalRConnectionState);
                    connection.onreconnecting(updateSignalRConnectionState);
                    connection.onclose(updateSignalRConnectionState);

                    await connection.start();
                    updateSignalRConnectionState();
                },
            }),
            {
                name: 'squidjam-state',
                partialize: (state) => ({ player: state.player }),
            },
        ),
    ),
);

useStore.getState().setupSignalR();

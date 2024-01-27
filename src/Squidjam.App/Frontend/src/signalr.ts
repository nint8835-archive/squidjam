import * as signalR from '@microsoft/signalr';
import { usePlayerStore } from './state/player';

export const connection = new signalR.HubConnectionBuilder()
    .withUrl(`/api/realtime?playerId=${usePlayerStore.getState().player}`)
    .build();

connection.start();

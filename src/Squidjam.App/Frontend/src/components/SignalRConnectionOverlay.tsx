import * as signalR from '@microsoft/signalr';
import { useStore } from '../store';

const connectingStates = [signalR.HubConnectionState.Connecting, signalR.HubConnectionState.Reconnecting];

function OverlayContainer({ children }: { children: React.ReactNode }) {
    return (
        <div className="bg-blend-o absolute left-0 top-0 z-50 flex h-full w-full items-center justify-center bg-zinc-800 bg-opacity-10 backdrop-blur-md">
            <div className="flex flex-col items-center space-y-2">{children}</div>
        </div>
    );
}

export default function SignalRConnectionOverlay() {
    const { signalRState, signalRConnectionError } = useStore();

    if (connectingStates.includes(signalRState)) {
        return (
            <OverlayContainer>
                <div className="h-24 w-24 animate-spin rounded-full border-t-2 border-sky-500"></div>
                <div className="text-2xl">{signalRState}</div>
            </OverlayContainer>
        );
    }

    if (signalRState === signalR.HubConnectionState.Disconnected) {
        return (
            <OverlayContainer>
                <div className="text-2xl text-red-600">Disconnected</div>
                <p className="whitespace-pre">
                    A connection to SignalR is required to play this game, but a connection failed to be established.
                    Refresh the page to try again.
                </p>
                {signalRConnectionError && <pre className="rounded-md bg-zinc-950 p-2">{signalRConnectionError}</pre>}
            </OverlayContainer>
        );
    }

    return null;
}

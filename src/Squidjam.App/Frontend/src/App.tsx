import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { RouterProvider, createBrowserRouter } from 'react-router-dom';
import { Toaster } from 'sonner';
import SignalRConnectionOverlay from './components/SignalRConnectionOverlay';
import { fetchGetGame, fetchPerformAction } from './queries/api/squidjamComponents';
import GamePage from './routes/Game';
import GameListPage from './routes/GameList';
import { useStore } from './store';

const queryClient = new QueryClient();

const router = createBrowserRouter([
    {
        path: '/',
        element: <GameListPage />,
    },
    {
        path: '/game/:gameId',
        element: <GamePage />,
        loader: async ({ params }) => {
            try {
                const joinResp = await fetchPerformAction({
                    pathParams: { gameId: params.gameId! },
                    body: {
                        type: 'AddPlayer',
                        player: useStore.getState().playerId,
                        name: useStore.getState().playerName,
                    },
                });

                useStore.setState({ currentGame: joinResp });
            } catch (err) {
                const stateResp = await fetchGetGame({ pathParams: { gameId: params.gameId! } });
                useStore.setState({ currentGame: stateResp });
            }

            return null;
        },
    },
]);

export default function App() {
    return (
        <QueryClientProvider client={queryClient}>
            <Toaster position="top-right" theme="dark" richColors />
            <SignalRConnectionOverlay />
            <RouterProvider router={router} />
            <ReactQueryDevtools initialIsOpen={false} buttonPosition="top-left" />
        </QueryClientProvider>
    );
}

import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { RouterProvider, createBrowserRouter } from 'react-router-dom';
import SignalRConnectionOverlay from './components/SignalRConnectionOverlay';
import GameList from './routes/GameList';

const queryClient = new QueryClient();

const router = createBrowserRouter([
    {
        path: '/',
        element: <GameList />,
    },
]);

export default function App() {
    return (
        <QueryClientProvider client={queryClient}>
            <SignalRConnectionOverlay />
            <RouterProvider router={router} />
            <ReactQueryDevtools initialIsOpen={false} />
        </QueryClientProvider>
    );
}

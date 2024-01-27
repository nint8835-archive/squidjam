import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { RouterProvider, createBrowserRouter } from 'react-router-dom';
import Test from './Test';
import SignalRConnectionOverlay from './components/SignalRConnectionOverlay';

const queryClient = new QueryClient();

const router = createBrowserRouter([
    {
        path: '/',
        element: <Test />,
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

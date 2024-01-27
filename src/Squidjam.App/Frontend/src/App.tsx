import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import Test from './Test';

const queryClient = new QueryClient();

export default function App() {
    return (
        <QueryClientProvider client={queryClient}>
            <Test />
            <ReactQueryDevtools initialIsOpen={false} />
        </QueryClientProvider>
    );
}

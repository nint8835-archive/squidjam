import react from '@vitejs/plugin-react';
import { defineConfig } from 'vite';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [react()],
    build: {
        outDir: '../wwwroot',
    },
    server: {
        proxy: {
            '/api': {
                target: 'http://localhost:5038',
                changeOrigin: true,
            },
        },
    },
});

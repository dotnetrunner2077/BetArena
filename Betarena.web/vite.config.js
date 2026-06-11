import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin()],
    server: {
        port: 50062,
        proxy: {
            '/bets': { target: 'http://localhost:5045', changeOrigin: true },
            '/stats': { target: 'http://localhost:5045', changeOrigin: true },
        },
    }
})

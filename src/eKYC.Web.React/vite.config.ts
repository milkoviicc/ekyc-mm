import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// In dev the React app talks to eKYC.Api through this proxy, so no CORS setup is needed on the API.
// Change VITE_API_PROXY_TARGET in .env.development if the API runs on another port.
export default defineConfig(({ mode }) => {
  const apiTarget = process.env.VITE_API_PROXY_TARGET ?? 'http://localhost:5124';
  return {
    plugins: [react()],
    server: {
      port: 3000,
      proxy: {
        '/api': { target: apiTarget, changeOrigin: true },
      },
    },
    build: {
      outDir: 'build',
      sourcemap: mode !== 'production',
    },
  };
});

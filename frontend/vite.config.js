import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    // Used only by "npm run dev". The React code always calls /api/...;
    // here Vite forwards those calls to the backend started with "dotnet run".
    // In Docker the same job is done by nginx (see nginx.conf).
    proxy: {
      '/api': 'http://localhost:5104',
    },
  },
})

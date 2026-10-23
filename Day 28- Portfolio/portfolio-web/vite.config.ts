import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    // While developing, /api calls go to the .NET API
    proxy: { '/api': 'http://localhost:5080' }
  },
  build: {
    // The production build lands in the API's wwwroot, so one .NET app serves everything
    outDir: '../Portfolio.Api/wwwroot',
    emptyOutDir: true
  }
})

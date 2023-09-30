import { defineConfig } from "vite"
import path from "path"
import defineManifest from "./manifest.config"
import react from "@vitejs/plugin-react"
import { crx } from "@crxjs/vite-plugin"

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react(), crx({ manifest: defineManifest })],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    port: 3040,
    strictPort: true,
    hmr: {
      clientPort: 3040,
    },
  },
})

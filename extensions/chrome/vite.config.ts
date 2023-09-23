import { defineConfig } from "vite"
import defineManifest from "./manifest.config"
import react from "@vitejs/plugin-react"
import { crx } from "@crxjs/vite-plugin"

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react(), crx({ manifest: defineManifest })],
  server: {
    port: 3000,
    strictPort: true,
    hmr: {
      clientPort: 3000,
    },
  },
})

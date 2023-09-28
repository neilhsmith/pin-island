import { defineConfig } from "vite"
import path from "path"
import react from "@vitejs/plugin-react"
import mkcert from "vite-plugin-mkcert"

const baseProxy = {
  target: "https://localhost:3001",
  secure: false,
}

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react(), mkcert()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    https: true,
    port: 3131,
    strictPort: true,

    proxy: {
      "/bff": baseProxy,
      "/signin-oidc": baseProxy,
      "/signout-callback-oidc": baseProxy,
      "/api": baseProxy,
    },
  },
})

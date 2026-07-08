import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// أثناء التطوير: يوجّه أي طلب /api إلى خادم ASP.NET Core
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      "/api": {
        target: "http://localhost:5080",
        changeOrigin: true,
      },
    },
  },
});

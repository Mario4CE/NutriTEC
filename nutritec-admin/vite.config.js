import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// =============================================================
// Configuración de Vite para la Vista Administrador (NutriTEC)
//
// El proxy redirige las llamadas que empiecen con /api hacia el
// backend real cuando esté corriendo localmente en desarrollo.
// Esto evita problemas de CORS mientras programamos.
// Cuando se despliegue a producción (Azure/AWS), esto no aplica:
// ahí se usa directamente la URL completa definida en
// src/config/api.js
// =============================================================
export default defineConfig({
  plugins: [react()],
  server: {
    port: 3001,
    proxy: {
      "/api/sql": {
        target: "http://localhost:5255",
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api\/sql/, "/api"),
      },
      "/api/mongo": {
        target: "http://localhost:5272",
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api\/mongo/, "/api"),
      },
    },
  },
});
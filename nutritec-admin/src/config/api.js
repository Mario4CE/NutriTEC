/**
 * api.js — Configuración central de las APIs de NutriTEC
 *
 * NutriTEC usa DOS backends distintos (según el enunciado del proyecto):
 *   1. API de SQL Server  → usuarios, nutricionistas, productos, planes,
 *      registros diarios, reporte de cobro, etc. (Entity Framework / C#)
 *   2. API de MongoDB     → retroalimentación tipo foro entre
 *      nutricionista y paciente.
 *
 * Cambia BASE_URL_SQL y BASE_URL_MONGO para apuntar a tus servidores
 * reales. Todos los componentes importan desde aquí — nunca escribas
 * una URL directamente dentro de un componente.
 *
 * Mientras el equipo no tenga las APIs listas, USE_MOCKS = true hace
 * que toda la app funcione con datos falsos (ver mockData.js), sin
 * tener que cambiar ningún componente cuando conectemos el backend real.
 */

export const USE_MOCKS = true;

export const BASE_URL_SQL = "/api/sql";
export const BASE_URL_MONGO = "/api/mongo";

/**
 * Endpoints agrupados por recurso. Así, si el backend cambia una ruta,
 * se corrige en un solo lugar.
 */
export const ENDPOINTS = {
  // ---------- Autenticación ----------
  auth: {
    adminLogin: () => `${BASE_URL_SQL}/auth/admin/login`,
  },

  // ---------- Administrador: aprobación de productos ----------
  productos: {
    pendientes: () => `${BASE_URL_SQL}/productos?aprobado=false`,
    aprobar: (idProducto) => `${BASE_URL_SQL}/productos/${idProducto}/aprobar`,
    rechazar: (idProducto) => `${BASE_URL_SQL}/productos/${idProducto}/rechazar`,
  },

  // ---------- Administrador: reporte de cobro ----------
  cobros: {
    reporte: (tipoCobro) => `${BASE_URL_SQL}/cobros/reporte?tipo=${tipoCobro}`,
  },

  // ---------- MongoDB: retroalimentación (no usado en vista admin) ----------
  retroalimentacion: {
    porUsuario: (idUsuario) => `${BASE_URL_MONGO}/retroalimentacion/usuario/${idUsuario}`,
  },
};

/**
 * Wrapper simple de fetch con manejo de errores consistente.
 * Cuando conectemos el backend real, todos los servicios (loginService,
 * productosService, etc.) usan esto por debajo — no hay que tocar
 * los componentes.
 */
export async function apiFetch(url, options = {}) {
  const res = await fetch(url, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });

  if (!res.ok) {
    const errorBody = await res.json().catch(() => ({}));
    throw new Error(errorBody.message || `Error ${res.status} al llamar ${url}`);
  }

  if (res.status === 204) return null;
  return res.json();
}
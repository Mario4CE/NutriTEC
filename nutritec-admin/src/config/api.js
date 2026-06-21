/**
 * api.js — Configuración central de las APIs de NutriTEC
 *
 * SQL Server API  → http://localhost:5255  (proxy: /api/sql → /api)
 * MongoDB API     → http://localhost:5272  (proxy: /api/mongo → /api)
 *
 * Cambiar USE_MOCKS a false cuando el backend esté corriendo.
 */

export const USE_MOCKS = true;

const SQL   = "/api/sql";
const MONGO = "/api/mongo";

export const ENDPOINTS = {
  auth: {
    login: () => `${SQL}/auth/login`,
  },
  administracion: {
    productosPendientes: ()    => `${SQL}/administracion/productos/pendientes`,
    aprobarProducto:    (id)   => `${SQL}/administracion/productos/${id}/aprobacion`,
    // GET ?montoBasePorPaciente=1&incluirSinPacientes=true
    // Devuelve lista plana de ReporteCobroNutricionistaResponse — el front agrupa por tipoCobro
    reporteCobro: (monto = 1, incluirSinPacientes = true) =>
      `${SQL}/administracion/reporte-cobro?montoBasePorPaciente=${monto}&incluirSinPacientes=${incluirSinPacientes}`,
  },
  productos: {
    eliminar: (id) => `${SQL}/productos/${id}`,
  },
  retroalimentacion: {
    porUsuario: (idUsuario) => `${MONGO}/retroalimentaciones/usuario/${idUsuario}`,
  },
};

/**
 * apiFetch — wrapper con JWT automático desde sessionStorage.
 */
export async function apiFetch(url, options = {}) {
  const session = sessionStorage.getItem("nutritec_admin_session");
  const token   = session ? JSON.parse(session).token : null;

  const res = await fetch(url, {
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    ...options,
  });

  if (!res.ok) {
    const body = await res.json().catch(() => ({}));
    throw new Error(body.message ?? `Error ${res.status} al llamar ${url}`);
  }

  if (res.status === 204) return null;
  return res.json();
}
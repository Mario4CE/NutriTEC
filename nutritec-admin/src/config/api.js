/**
 * api.js — Configuración central de las APIs de NutriTEC
 *
 * SQL Server API  → http://localhost:5255  (proxy: /api/sql → /api)
 * MongoDB API     → http://localhost:5272  (proxy: /api/mongo → /api)
 *
 * Cambiar USE_MOCKS a false cuando el backend esté corriendo.
 */

export const USE_MOCKS = false;

const SQL   = "/api/sql";
const MONGO = "/api/mongo";

export const ENDPOINTS = {

  // ── Auth ──────────────────────────────────────────────────
  // POST { Correo, Contrasena }  →  LoginResponse
  auth: {
    login:  () => `${SQL}/auth/login`,
    me:     () => `${SQL}/auth/me`,          // GET — valida el token actual
  },

  // ── Administración (EF Core) ──────────────────────────────
  administracion: {
    productosPendientes: ()    => `${SQL}/administracion/productos/pendientes`,
    aprobarProducto:    (id)   => `${SQL}/administracion/productos/${id}/aprobacion`,      // PUT → 204
    imc:        (kg, cm)       => `${SQL}/administracion/imc?pesoKg=${kg}&estaturaCm=${cm}`,
  },

  // ── Objetos SQL (SP + Funciones directas) ─────────────────
  // Estos endpoints llaman al SP/función real de SQL Server.
  // Usamos estos para el reporte de cobro y aprobación porque
  // el enunciado pide explícitamente que se use el SP.
  sp: {
    reporteCobro:   (monto = 1, incluirSinPacientes = true) =>
      `${SQL}/sql-programable/stored-procedures/reporte-cobro-nutricionistas?montoBasePorPaciente=${monto}&incluirSinPacientes=${incluirSinPacientes}`,
    aprobarProductoSP: (id) =>
      `${SQL}/sql-programable/stored-procedures/productos/${id}/aprobacion`,               // PUT → ProductoAprobadoSqlResponse
    calcularImc:    (kg, cm) =>
      `${SQL}/sql-programable/functions/imc?pesoKg=${kg}&estaturaCm=${cm}`,
    totalCaloriasPlan: (idPlan) =>
      `${SQL}/sql-programable/functions/planes/${idPlan}/total-calorias`,
  },

  // ── Productos (CRUD) ──────────────────────────────────────
  productos: {
    listar:           ()     => `${SQL}/productos`,
    buscar:  (nombre)        => `${SQL}/productos/buscar?nombre=${encodeURIComponent(nombre)}`,
    porId:   (id)            => `${SQL}/productos/${id}`,
    porCodigo: (codigo)      => `${SQL}/productos/codigo-barras/${codigo}`,
    eliminar:  (id)          => `${SQL}/productos/${id}`,                                  // DELETE
  },

  // ── MongoDB: retroalimentación ────────────────────────────
  retroalimentacion: {
    porUsuario: (idUsuario)  => `${MONGO}/retroalimentaciones/usuario/${idUsuario}`,
  },
};

/**
 * apiFetch — wrapper de fetch con JWT automático.
 * Lee el token de sessionStorage y lo manda en Authorization: Bearer.
 */
export async function apiFetch(url, options = {}) {
  const raw   = sessionStorage.getItem("nutritec_admin_session");
  const token = raw ? JSON.parse(raw).token : null;

  const res = await fetch(url, {
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    ...options,
  });

  if (!res.ok) {
    const body = await res.json().catch(() => ({}));
    throw new Error(body.message ?? `Error ${res.status}`);
  }

  if (res.status === 204) return null;
  return res.json();
}
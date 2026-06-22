/**
 * api.js — Configuración central de las APIs de NutriTEC
 * Vista Cliente
 *
 * Para cambiar de servidor, solo modificá las URLs acá.
 * Ningún otro archivo tiene URLs hardcodeadas.
 *
 * SQL Server API  → http://localhost:5255
 * MongoDB API     → http://localhost:5272
 */

const API_CONFIG = {
  USE_MOCKS: false,
  SQL: "http://localhost:5255/api",
  MONGO: "http://localhost:5272/api",
};

const ENDPOINTS = {
  // ── Autenticación ──────────────────────────────────────────
  // POST { Correo, Contrasena } → LoginResponse
  // POST { Nombre, Apellidos, Correo, Contrasena, ... } → LoginResponse
  auth: {
    login:            () => `${API_CONFIG.SQL}/auth/login`,
    registrarCliente: () => `${API_CONFIG.SQL}/auth/registrar-cliente`,
  },

  // ── Productos (aprobados) ──────────────────────────────────
  // GET /productos              → lista todos los aprobados
  // GET /productos/buscar?nombre=X → busca por nombre
  // GET /productos/codigo-barras/X → busca por código
  // POST /productos             → crea producto (queda pendiente)
  productos: {
    listar:    ()       => `${API_CONFIG.SQL}/productos`,
    buscar:    (nombre) => `${API_CONFIG.SQL}/productos/buscar?nombre=${encodeURIComponent(nombre)}`,
    porCodigo: (codigo) => `${API_CONFIG.SQL}/productos/codigo-barras/${encodeURIComponent(codigo)}`,
    crear:     ()       => `${API_CONFIG.SQL}/productos`,
  },

  // ── Objetos SQL (SP + Funciones) ───────────────────────────
  sp: {
    // POST { idPlan, idUsuario, fechaInicio, fechaFin }
    asignarPlan:     () => `${API_CONFIG.SQL}/sql-programable/stored-procedures/asignar-plan-paciente`,
    // POST { idUsuario, fecha, pesoKg, estaturaCm, cintura, cuello, caderas, pctMusculo, pctGrasa }
    registrarMedida: () => `${API_CONFIG.SQL}/sql-programable/stored-procedures/registrar-medida-usuario`,
    // GET ?pesoKg=X&estaturaCm=Y → { imc }
    calcularImc:     (kg, cm) => `${API_CONFIG.SQL}/sql-programable/functions/imc?pesoKg=${kg}&estaturaCm=${cm}`,
  },

  // ── MongoDB: retroalimentación ─────────────────────────────
  // GET  /retroalimentaciones/usuario/:id
  // POST /retroalimentaciones → crear retroalimentación
  // POST /retroalimentaciones/:id/respuestas → responder
  retroalimentacion: {
    porUsuario: (idUsuario) => `${API_CONFIG.MONGO}/retroalimentaciones/usuario/${idUsuario}`,
    crear:      ()          => `${API_CONFIG.MONGO}/retroalimentaciones`,
    responder:  (id)        => `${API_CONFIG.MONGO}/retroalimentaciones/${id}/respuestas`,
  },
};

/**
 * apiFetch — wrapper de fetch con JWT automático.
 * Lee el token de sessionStorage y lo manda en Authorization: Bearer.
 */
async function apiFetch(url, options = {}) {
  const raw   = sessionStorage.getItem("nutritec_session");
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
    throw new Error(body.message ?? body.mensaje ?? `Error ${res.status}`);
  }

  if (res.status === 204) return null;
  return res.json();
}

/**
 * getSession — devuelve la sesión actual o null
 */
function getSession() {
  const raw = sessionStorage.getItem("nutritec_session");
  return raw ? JSON.parse(raw) : null;
}

/**
 * setSession — guarda la sesión
 */
function setSession(session) {
  sessionStorage.setItem("nutritec_session", JSON.stringify(session));
}

/**
 * clearSession — elimina la sesión
 */
function clearSession() {
  sessionStorage.removeItem("nutritec_session");
}
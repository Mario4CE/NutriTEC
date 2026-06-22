/**
 * api.js — Configuración central de las APIs de NutriTEC
 * Vista Cliente
 */

const API_CONFIG = {
  USE_MOCKS: false,
  SQL:   "http://localhost:5255/api",
  MONGO: "http://localhost:5272/api",
};

const ENDPOINTS = {
  auth: {
    login:            () => `${API_CONFIG.SQL}/auth/login`,
    registrarCliente: () => `${API_CONFIG.SQL}/auth/registrar-cliente`,
    me:               () => `${API_CONFIG.SQL}/auth/me`,
  },
  productos: {
    listar:    ()       => `${API_CONFIG.SQL}/productos`,
    buscar:    (nombre) => `${API_CONFIG.SQL}/productos/buscar?nombre=${encodeURIComponent(nombre)}`,
    porCodigo: (codigo) => `${API_CONFIG.SQL}/productos/codigo-barras/${encodeURIComponent(codigo)}`,
    crear:     ()       => `${API_CONFIG.SQL}/productos`,
  },
  planes: {
    porUsuario: (idUsuario) => `${API_CONFIG.SQL}/planes/usuario/${idUsuario}`,
  },
  registros: {
    crear:      ()           => `${API_CONFIG.SQL}/registros-diarios`,
    porUsuario: (idUsuario)  => `${API_CONFIG.SQL}/registros-diarios/usuario/${idUsuario}`,
    eliminar:   (idRegistro) => `${API_CONFIG.SQL}/registros-diarios/${idRegistro}`,
  },
  recetas: {
    crear:      ()          => `${API_CONFIG.SQL}/recetas`,
    porUsuario: (idUsuario) => `${API_CONFIG.SQL}/recetas/usuario/${idUsuario}`,
    detalle:    (idUsuario) => `${API_CONFIG.SQL}/recetas/usuario/${idUsuario}/detalle`,
  },
  medidas: {
    registrar:  (idUsuario) => `${API_CONFIG.SQL}/medidas/usuario/${idUsuario}`,
    porUsuario: (idUsuario) => `${API_CONFIG.SQL}/medidas/usuario/${idUsuario}`,
  },
  sp: {
    calcularImc: (kg, cm) => `${API_CONFIG.SQL}/sql-programable/functions/imc?pesoKg=${kg}&estaturaCm=${cm}`,
  },
  // MongoDB — retroalimentación entre paciente y nutricionista
  retroalimentacion: {
    crear:          ()           => `${API_CONFIG.MONGO}/retroalimentaciones`,
    porPaciente:    (idPaciente) => `${API_CONFIG.MONGO}/retroalimentaciones/pacientes/${idPaciente}`,
    responder:      (id)         => `${API_CONFIG.MONGO}/retroalimentaciones/${id}/mensajes`,
  },
};

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

function getSession() {
  const raw = sessionStorage.getItem("nutritec_session");
  return raw ? JSON.parse(raw) : null;
}

function setSession(session) {
  sessionStorage.setItem("nutritec_session", JSON.stringify(session));
}

function clearSession() {
  sessionStorage.removeItem("nutritec_session");
}
const API_SQL_BASE_URL = "http://localhost:5255/api";
const API_MONGO_BASE_URL = "http://localhost:5272/api";

function obtenerToken() {
    return localStorage.getItem("nutritec_token");
}

function guardarSesion(respuesta) {
    localStorage.setItem("nutritec_token", respuesta.token);
    localStorage.setItem("nutritec_usuario", JSON.stringify({
        id: respuesta.id,
        nombre: respuesta.nombre,
        correo: respuesta.correo,
        tipoUsuario: respuesta.tipoUsuario
    }));
}

function obtenerUsuarioActual() {
    const datos = localStorage.getItem("nutritec_usuario");
    return datos ? JSON.parse(datos) : null;
}

function cerrarSesion() {
    localStorage.removeItem("nutritec_token");
    localStorage.removeItem("nutritec_usuario");
    window.location.href = "login.html";
}

function requiereAutenticacion() {
    if (!obtenerToken()) {
        window.location.href = "login.html";
    }
}

async function realizarPeticion(baseUrl, ruta, opciones = {}) {
    const token = obtenerToken();
    const headers = {
        "Content-Type": "application/json",
        ...(opciones.headers || {})
    };

    if (token) {
        headers["Authorization"] = `Bearer ${token}`;
    }

    const respuesta = await fetch(`${baseUrl}${ruta}`, {
        ...opciones,
        headers
    });

    if (respuesta.status === 401) {
        cerrarSesion();
        throw new Error("Sesión expirada.");
    }

    const cuerpo = await respuesta.json().catch(() => null);

    if (!respuesta.ok) {
        const mensaje = cuerpo?.mensaje || cuerpo?.message || "Ocurrió un error en la solicitud.";
        throw new Error(mensaje);
    }

    return cuerpo;
}

function apiFetch(ruta, opciones = {}) {
    return realizarPeticion(API_SQL_BASE_URL, ruta, opciones);
}

function apiMongoFetch(ruta, opciones = {}) {
    return realizarPeticion(API_MONGO_BASE_URL, ruta, opciones);
}
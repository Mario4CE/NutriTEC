requiereAutenticacion();

const usuarioActual = obtenerUsuarioActual();
const params = new URLSearchParams(window.location.search);
const idPaciente = params.get("idPaciente");

const idPacienteMostrado = document.getElementById("idPacienteMostrado");
const contenedorRegistroDiario = document.getElementById("contenedorRegistroDiario");
const hiloMensajes = document.getElementById("hiloMensajes");
const formMensaje = document.getElementById("formMensaje");
const alertaMensajeSeguimiento = document.getElementById("alertaMensaje");

let idRetroalimentacionActual = null;

document.addEventListener("DOMContentLoaded", async () => {
    if (!idPaciente) {
        mostrarMensajeSeguimiento("No se especificó un paciente.", "danger");
        return;
    }

    idPacienteMostrado.textContent = idPaciente;
    await Promise.all([cargarRegistroDiario(), cargarConversacion()]);
});

async function cargarRegistroDiario() {
    if (!contenedorRegistroDiario) return;

    try {
        const respuesta = await apiFetch(`/registro-diario/paciente/${idPaciente}`);
        const registros = respuesta.data;

        if (!registros || registros.length === 0) {
            contenedorRegistroDiario.innerHTML = `<p class="text-muted small">El paciente aún no tiene registros diarios.</p>`;
            return;
        }

        const porFecha = registros.reduce((agrupado, registro) => {
            const fecha = new Date(registro.fecha).toLocaleDateString();
            if (!agrupado[fecha]) agrupado[fecha] = [];
            agrupado[fecha].push(registro);
            return agrupado;
        }, {});

        contenedorRegistroDiario.innerHTML = Object.entries(porFecha).map(([fecha, items]) => `
            <div class="mb-2">
                <div class="small fw-bold text-muted">${fecha}</div>
                ${items.map(item => `
                    <div class="small ps-2">• ${item.tipo_comida ?? item.tipoComida ?? ''}</div>
                `).join("")}
            </div>
        `).join("");
    } catch (error) {
        if (contenedorRegistroDiario) {
            contenedorRegistroDiario.innerHTML = `<p class="text-muted small">No se pudo cargar el registro diario.</p>`;
        }
    }
}

async function cargarConversacion() {
    try {
        const respuesta = await apiMongoFetch(`/retroalimentaciones/pacientes/${idPaciente}`);
        const foros = respuesta.data;

        if (!foros || foros.length === 0) {
            hiloMensajes.innerHTML = `<p class="text-muted">Aún no hay conversación con este paciente. Escriba el primer mensaje abajo.</p>`;
            idRetroalimentacionActual = null;
            return;
        }

        const foro = foros[0];
        idRetroalimentacionActual = foro.id;
        mostrarMensajes(foro.mensajes);
    } catch (error) {
        hiloMensajes.innerHTML = `<p class="text-danger">No se pudo cargar la conversación.</p>`;
    }
}

function mostrarMensajes(mensajes) {
    if (!mensajes || mensajes.length === 0) {
        hiloMensajes.innerHTML = `<p class="text-muted">Sin mensajes todavía.</p>`;
        return;
    }

    hiloMensajes.innerHTML = mensajes.map(mensaje => `
        <div class="mb-2 p-2 rounded ${mensaje.autor === usuarioActual.nombre ? 'bg-success bg-opacity-10 text-end' : 'bg-light'}">
            <div class="small fw-bold">${mensaje.autor}</div>
            <div>${mensaje.mensaje}</div>
            <div class="small text-muted">${new Date(mensaje.fechaUtc).toLocaleString()}</div>
        </div>
    `).join("");

    hiloMensajes.scrollTop = hiloMensajes.scrollHeight;
}

if (formMensaje) {
    formMensaje.addEventListener("submit", async (evento) => {
        evento.preventDefault();
        const texto = document.getElementById("textoMensaje").value.trim();

        if (!texto) return;

        try {
            if (idRetroalimentacionActual) {
                await apiMongoFetch(`/retroalimentaciones/${idRetroalimentacionActual}/mensajes`, {
                    method: "POST",
                    body: JSON.stringify({ autor: usuarioActual.nombre, mensaje: texto })
                });
            } else {
                await apiMongoFetch("/retroalimentaciones", {
                    method: "POST",
                    body: JSON.stringify({
                        idPaciente,
                        idNutricionista: usuarioActual.id,
                        autor: usuarioActual.nombre,
                        mensaje: texto
                    })
                });
            }

            document.getElementById("textoMensaje").value = "";
            await cargarConversacion();
        } catch (error) {
            mostrarMensajeSeguimiento(error.message, "danger");
        }
    });
}

function mostrarMensajeSeguimiento(texto, tipo) {
    if (!alertaMensajeSeguimiento) return;
    alertaMensajeSeguimiento.textContent = texto;
    alertaMensajeSeguimiento.className = `alert alert-${tipo}`;
    alertaMensajeSeguimiento.classList.remove("d-none");
    setTimeout(() => alertaMensajeSeguimiento.classList.add("d-none"), 4000);
}
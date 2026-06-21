requiereAutenticacion();

const usuario = obtenerUsuarioActual();
const params = new URLSearchParams(window.location.search);
const idPaciente = params.get("idPaciente");

const idPacienteMostrado = document.getElementById("idPacienteMostrado");
const hiloMensajes = document.getElementById("hiloMensajes");
const formMensaje = document.getElementById("formMensaje");
const alertaMensajeSeguimisento = document.getElementById("alertaMensaje");

let idRetroalimentacionActual = null;

document.addEventListener("DOMContentLoaded", async () => {
    if (!idPaciente) {
        mostrarMensajeSeguimiento("No se especificó un paciente.", "danger");
        return;
    }

    idPacienteMostrado.textContent = idPaciente;
    await cargarConversacion();
});

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
        <div class="mb-2 p-2 rounded ${mensaje.autor === usuario.nombre ? 'bg-success bg-opacity-10 text-end' : 'bg-light'}">
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
                    body: JSON.stringify({ autor: usuario.nombre, mensaje: texto })
                });
            } else {
                await apiMongoFetch("/retroalimentaciones", {
                    method: "POST",
                    body: JSON.stringify({
                        idPaciente,
                        idNutricionista: usuario.id,
                        autor: usuario.nombre,
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
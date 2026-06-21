const formBusqueda = document.getElementById("formBusqueda");
const resultadosBusqueda = document.getElementById("resultadosBusqueda");
const listaPacientes = document.getElementById("listaPacientes");
const alertaMensaje = document.getElementById("alertaMensaje");

document.addEventListener("DOMContentLoaded", cargarPacientes);

if (formBusqueda) {
    formBusqueda.addEventListener("submit", async (evento) => {
        evento.preventDefault();
        const criterio = document.getElementById("criterioBusqueda").value.trim();

        if (!criterio) {
            return;
        }

        try {
            const respuesta = await apiFetch(`/pacientes/buscar-clientes?criterio=${encodeURIComponent(criterio)}`);
            mostrarResultadosBusqueda(respuesta.data);
        } catch (error) {
            mostrarMensaje(error.message, "danger");
        }
    });
}

function mostrarResultadosBusqueda(clientes) {
    if (!clientes || clientes.length === 0) {
        resultadosBusqueda.innerHTML = `<p class="text-muted">No se encontraron clientes.</p>`;
        return;
    }

    const filas = clientes.map(cliente => `
        <tr>
            <td>${cliente.nombre} ${cliente.apellidos}</td>
            <td>${cliente.correo}</td>
            <td class="text-end">
                <button class="btn btn-sm btn-success" onclick="asociarPaciente('${cliente.id}')">Asociar como paciente</button>
            </td>
        </tr>
    `).join("");

    resultadosBusqueda.innerHTML = `
        <table class="table table-sm align-middle">
            <thead>
                <tr><th>Nombre</th><th>Correo</th><th></th></tr>
            </thead>
            <tbody>${filas}</tbody>
        </table>
    `;
}

async function asociarPaciente(idCliente) {
    try {
        await apiFetch("/pacientes", {
            method: "POST",
            body: JSON.stringify({ idCliente })
        });

        mostrarMensaje("Paciente asociado correctamente.", "success");
        resultadosBusqueda.innerHTML = "";
        document.getElementById("criterioBusqueda").value = "";
        cargarPacientes();
    } catch (error) {
        mostrarMensaje(error.message, "danger");
    }
}

async function cargarPacientes() {
    try {
        const respuesta = await apiFetch("/pacientes");
        mostrarListaPacientes(respuesta.data);
    } catch (error) {
        listaPacientes.innerHTML = `<p class="text-danger">No se pudieron cargar los pacientes.</p>`;
    }
}

function mostrarListaPacientes(pacientes) {
    if (!pacientes || pacientes.length === 0) {
        listaPacientes.innerHTML = `<p class="text-muted">Aún no tiene pacientes asociados.</p>`;
        return;
    }

    const filas = pacientes.map(paciente => `
        <tr>
            <td>${paciente.idPaciente}</td>
            <td>${new Date(paciente.fechaAsociacionUtc).toLocaleDateString()}</td>
            <td class="text-end">
                <a href="seguimiento.html?idPaciente=${paciente.idPaciente}" class="btn btn-sm btn-success">Seguimiento</a>
                <button class="btn btn-sm btn-outline-danger" onclick="desasociarPaciente('${paciente.id}')">Quitar</button>
            </td>
        </tr>
    `).join("");

    listaPacientes.innerHTML = `
        <table class="table table-sm align-middle">
            <thead>
                <tr><th>Paciente</th><th>Fecha asociación</th><th></th></tr>
            </thead>
            <tbody>${filas}</tbody>
        </table>
    `;
}

async function desasociarPaciente(id) {
    if (!confirm("¿Está seguro de quitar este paciente?")) {
        return;
    }

    try {
        await apiFetch(`/pacientes/${id}`, { method: "DELETE" });
        mostrarMensaje("Paciente eliminado.", "success");
        cargarPacientes();
    } catch (error) {
        mostrarMensaje(error.message, "danger");
    }
}

function mostrarMensaje(texto, tipo) {
    if (!alertaMensaje) return;
    alertaMensaje.textContent = texto;
    alertaMensaje.className = `alert alert-${tipo}`;
    alertaMensaje.classList.remove("d-none");
    setTimeout(() => alertaMensaje.classList.add("d-none"), 4000);
}
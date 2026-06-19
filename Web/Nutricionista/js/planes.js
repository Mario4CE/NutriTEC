const tiemposComidaContenedor = document.getElementById("tiemposComida");
const btnAgregarItem = document.getElementById("btnAgregarItem");
const formPlan = document.getElementById("formPlan");
const plantillaItem = document.getElementById("plantillaItemPlan");
const listaPlanes = document.getElementById("listaPlanes");
const alertaMensajePlanes = document.getElementById("alertaMensaje");
const totalCaloriasEstimado = document.getElementById("totalCaloriasEstimado");

document.addEventListener("DOMContentLoaded", () => {
    agregarItemPlan();
    cargarPlanes();
});

if (btnAgregarItem) {
    btnAgregarItem.addEventListener("click", () => agregarItemPlan());
}

function agregarItemPlan() {
    const clon = plantillaItem.content.cloneNode(true);
    const fila = clon.querySelector(".item-plan");

    fila.querySelector(".btn-quitar-item").addEventListener("click", () => {
        fila.remove();
        actualizarTotalEstimado();
    });

    fila.querySelector(".campo-porciones").addEventListener("input", actualizarTotalEstimado);
    fila.querySelector(".campo-codigo-producto").addEventListener("blur", async (evento) => {
        await actualizarCaloriasItem(fila, evento.target.value.trim());
    });

    tiemposComidaContenedor.appendChild(clon);
}

async function actualizarCaloriasItem(fila, codigoBarras) {
    if (!codigoBarras) return;

    try {
        const respuesta = await apiFetch(`/productos/codigo-barras/${encodeURIComponent(codigoBarras)}`);
        fila.dataset.calorias = respuesta.data.calorias;
        fila.dataset.idProducto = respuesta.data.id;
        fila.dataset.nombreProducto = respuesta.data.nombre;
    } catch (error) {
        fila.dataset.calorias = "";
        fila.dataset.idProducto = "";
        mostrarMensajePlanes(`Producto no encontrado para el código ${codigoBarras}.`, "warning");
    }

    actualizarTotalEstimado();
}

function actualizarTotalEstimado() {
    let total = 0;
    document.querySelectorAll(".item-plan").forEach(fila => {
        const calorias = parseFloat(fila.dataset.calorias) || 0;
        const porciones = parseFloat(fila.querySelector(".campo-porciones").value) || 0;
        total += calorias * porciones;
    });
    totalCaloriasEstimado.textContent = total.toFixed(1);
}

if (formPlan) {
    formPlan.addEventListener("submit", async (evento) => {
        evento.preventDefault();

        const nombre = document.getElementById("nombrePlan").value.trim();
        const filas = document.querySelectorAll(".item-plan");
        const items = [];

        for (const fila of filas) {
            const idProducto = fila.dataset.idProducto;
            const tiempoComida = parseInt(fila.querySelector(".campo-tiempo-comida").value, 10);
            const porciones = parseFloat(fila.querySelector(".campo-porciones").value);

            if (!idProducto) {
                mostrarMensajePlanes("Verifique que todos los productos tengan un código de barras válido.", "danger");
                return;
            }

            items.push({ tiempoComida, idProducto, porciones });
        }

        if (items.length === 0) {
            mostrarMensajePlanes("El plan debe incluir al menos un producto.", "danger");
            return;
        }

        try {
            await apiFetch("/planes", {
                method: "POST",
                body: JSON.stringify({ nombre, items })
            });

            mostrarMensajePlanes("Plan creado correctamente.", "success");
            formPlan.reset();
            tiemposComidaContenedor.innerHTML = "";
            agregarItemPlan();
            actualizarTotalEstimado();
            cargarPlanes();
        } catch (error) {
            mostrarMensajePlanes(error.message, "danger");
        }
    });
}

async function cargarPlanes() {
    try {
        const respuesta = await apiFetch("/planes");
        mostrarListaPlanes(respuesta.data);
    } catch (error) {
        listaPlanes.innerHTML = `<p class="text-danger">No se pudieron cargar los planes.</p>`;
    }
}

const nombresTiempoComida = {
    1: "Desayuno",
    2: "Merienda mañana",
    3: "Almuerzo",
    4: "Merienda tarde",
    5: "Cena"
};

function mostrarListaPlanes(planes) {
    if (!planes || planes.length === 0) {
        listaPlanes.innerHTML = `<p class="text-muted">Aún no ha creado planes.</p>`;
        return;
    }

    listaPlanes.innerHTML = planes.map(plan => `
        <div class="card mb-2">
            <div class="card-body">
                <div class="d-flex justify-content-between align-items-center">
                    <h3 class="h6 mb-0">${plan.nombre}</h3>
                    <span class="badge bg-success">${plan.caloriasTotales.toFixed(1)} kcal</span>
                </div>
                <ul class="list-unstyled mt-2 mb-0 small text-muted">
                    ${plan.items.map(item => `
                        <li>${nombresTiempoComida[item.tiempoComida]}: ${item.nombreProducto} (${item.porciones} porción/es) - ${item.caloriasAportadas.toFixed(1)} kcal</li>
                    `).join("")}
                </ul>
            </div>
        </div>
    `).join("");
}

function mostrarMensajePlanes(texto, tipo) {
    if (!alertaMensajePlanes) return;
    alertaMensajePlanes.textContent = texto;
    alertaMensajePlanes.className = `alert alert-${tipo}`;
    alertaMensajePlanes.classList.remove("d-none");
    setTimeout(() => alertaMensajePlanes.classList.add("d-none"), 4000);
}
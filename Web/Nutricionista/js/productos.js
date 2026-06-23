const formProducto = document.getElementById("formProducto");
const alertaMensajeProducto = document.getElementById("alertaMensaje");

if (formProducto) {
    formProducto.addEventListener("submit", async (evento) => {
        evento.preventDefault();

        const datos = {
            nombre: document.getElementById("nombre").value.trim(),
            codigoBarras: document.getElementById("codigoBarras").value.trim(),
            porcionGramosMililitros: parseFloat(document.getElementById("porcionGramosMililitros").value),
            calorias: parseFloat(document.getElementById("calorias").value),
            proteinas: parseFloat(document.getElementById("proteinas").value),
            carbohidratos: parseFloat(document.getElementById("carbohidratos").value),
            grasas: parseFloat(document.getElementById("grasas").value),
            sodioMiligramos: parseFloat(document.getElementById("sodioMiligramos").value),
            calcioMiligramos: parseFloat(document.getElementById("calcioMiligramos").value) || null,
            hierroMiligramos: parseFloat(document.getElementById("hierroMiligramos").value) || null,
            vitaminas: document.getElementById("vitaminas").value.trim() || null
        };

        try {
            await apiFetch("/productos", {
                method: "POST",
                body: JSON.stringify(datos)
            });

            mostrarMensajeProducto("Producto agregado. Queda pendiente de aprobación.", "success");
            formProducto.reset();
        } catch (error) {
            mostrarMensajeProducto(error.message, "danger");
        }
    });
}

function mostrarMensajeProducto(texto, tipo) {
    if (!alertaMensajeProducto) return;
    alertaMensajeProducto.textContent = texto;
    alertaMensajeProducto.className = `alert alert-${tipo}`;
    alertaMensajeProducto.classList.remove("d-none");
    setTimeout(() => alertaMensajeProducto.classList.add("d-none"), 4000);
}
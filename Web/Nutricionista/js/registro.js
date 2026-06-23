const formRegistro = document.getElementById("formRegistro");
const alertaErrorRegistro = document.getElementById("alertaError");
const alertaExitoRegistro = document.getElementById("alertaExito");

if (formRegistro) {
    formRegistro.addEventListener("submit", async (evento) => {
        evento.preventDefault();
        ocultarAlertasRegistro();

        const datos = {
            nombre: document.getElementById("nombre").value.trim(),
            apellidos: document.getElementById("apellidos").value.trim(),
            cedula: document.getElementById("cedula").value.trim(),
            codigoNutricionista: document.getElementById("codigoNutricionista").value.trim(),
            edad: parseInt(document.getElementById("edad").value, 10),
            fechaNacimiento: document.getElementById("fechaNacimiento").value,
            peso: parseFloat(document.getElementById("peso").value),
            imc: parseFloat(document.getElementById("imc").value),
            direccion: document.getElementById("direccion").value.trim(),
            tarjetaCredito: document.getElementById("tarjetaCredito").value.trim(),
            tipoCobro: document.getElementById("tipoCobro").value,
            correo: document.getElementById("correo").value.trim(),
            contrasena: document.getElementById("contrasena").value
        };

        try {
            await apiFetch("/auth/registrar-nutricionista", {
                method: "POST",
                body: JSON.stringify(datos)
            });

            mostrarExitoRegistro("Registro exitoso. Ya puede iniciar sesión.");
            formRegistro.reset();
            setTimeout(() => {
                window.location.href = "login.html";
            }, 2000);
        } catch (error) {
            mostrarErrorRegistro(error.message);
        }
    });
}

function mostrarErrorRegistro(mensaje) {
    if (alertaErrorRegistro) {
        alertaErrorRegistro.textContent = mensaje;
        alertaErrorRegistro.classList.remove("d-none");
    }
}

function mostrarExitoRegistro(mensaje) {
    if (alertaExitoRegistro) {
        alertaExitoRegistro.textContent = mensaje;
        alertaExitoRegistro.classList.remove("d-none");
    }
}

function ocultarAlertasRegistro() {
    if (alertaErrorRegistro) alertaErrorRegistro.classList.add("d-none");
    if (alertaExitoRegistro) alertaExitoRegistro.classList.add("d-none");
}
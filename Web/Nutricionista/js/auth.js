const formLogin = document.getElementById("formLogin");
const alertaError = document.getElementById("alertaError");

if (formLogin) {
    formLogin.addEventListener("submit", async (evento) => {
        evento.preventDefault();
        ocultarError();

        const correo = document.getElementById("correo").value.trim();
        const contrasena = document.getElementById("contrasena").value;

        try {
            const respuesta = await apiFetch("/auth/login", {
                method: "POST",
                body: JSON.stringify({ correo, contrasena })
            });

            if (respuesta.tipoUsuario !== "Nutricionista") {
                mostrarError("Esta cuenta no corresponde a un nutricionista.");
                return;
            }

            guardarSesion(respuesta);
            window.location.href = "dashboard.html";
        } catch (error) {
            mostrarError(error.message);
        }
    });
}

function mostrarError(mensaje) {
    if (alertaError) {
        alertaError.textContent = mensaje;
        alertaError.classList.remove("d-none");
    }
}

function ocultarError() {
    if (alertaError) {
        alertaError.classList.add("d-none");
    }
}
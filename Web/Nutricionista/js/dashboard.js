requiereAutenticacion();

const usuario = obtenerUsuarioActual();
const nombreUsuario = document.getElementById("nombreUsuario");

if (usuario && nombreUsuario) {
    nombreUsuario.textContent = usuario.nombre;
}
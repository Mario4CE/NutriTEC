namespace NutriTec.Application.Autenticacion;

public sealed record UsuarioTokenAutenticacion(
    string IdUsuario,
    string Nombre,
    string Correo,
    string TipoUsuario);

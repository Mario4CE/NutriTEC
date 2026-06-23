namespace NutriTec.Domain.Pacientes;

/*
 * Descripción:
 * Representa un resumen de solo lectura de un usuario, usado para mostrar resultados
 * de búsqueda al nutricionista antes de asociarlo como paciente.
 *
 * Entradas:
 * Recibe los datos básicos del usuario provenientes del repositorio de autenticación.
 *
 * Salidas:
 * Permite identificar al usuario sin exponer datos sensibles como la contraseña.
 *
 * Restricciones:
 * No se persiste; es un objeto de proyección, no una entidad con ciclo de vida propio.
 */
public sealed class ClienteResumen
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Apellidos { get; init; } = string.Empty;
    public string Correo { get; init; } = string.Empty;
}
namespace NutriTec.Contracts.Retroalimentaciones;

/*
 * Descripción:
 * Define los datos requeridos para abrir un foro de retroalimentación.
 *
 * Entradas:
 * Recibe los identificadores del paciente y nutricionista, el autor y el mensaje inicial.
 *
 * Salidas:
 * Transporta la solicitud desde la API hacia la capa de aplicación.
 *
 * Restricciones:
 * La capa de aplicación valida identificadores, autor y contenido antes de persistirlos.
 */
public sealed record CrearRetroalimentacionRequest(
    Guid IdPaciente,
    Guid IdNutricionista,
    string Autor,
    string Mensaje);

namespace NutriTec.Contracts.Pacientes;

/*
 * Descripción:
 * DTO de solicitud para que un nutricionista asocie un cliente como su paciente.
 *
 * Entradas:
 * Recibe el identificador del cliente a asociar.
 *
 * Salidas:
 * No aplica.
 *
 * Restricciones:
 * El identificador del cliente es obligatorio.
 */
public sealed record AsociarPacienteRequest(
    Guid IdCliente);

/*
 * Descripción:
 * DTO de salida que representa un paciente asociado a un nutricionista.
 *
 * Entradas:
 * No aplica.
 *
 * Salidas:
 * Expone el identificador de la asociación, el cliente, el nutricionista y la fecha de asociación.
 *
 * Restricciones:
 * No aplica.
 */
public sealed record PacienteNutricionistaResponse(
    Guid Id,
    Guid IdNutricionista,
    Guid IdPaciente,
    DateTime FechaAsociacionUtc);

/*
 * Descripción:
 * DTO de salida para resultados de búsqueda de clientes candidatos a ser pacientes.
 *
 * Entradas:
 * No aplica.
 *
 * Salidas:
 * Expone los datos básicos del cliente necesarios para identificarlo en la búsqueda.
 *
 * Restricciones:
 * No aplica.
 */
public sealed record ClienteBusquedaResponse(
    Guid Id,
    string Nombre,
    string Apellidos,
    string Correo);
using NutriTec.Contracts.Pacientes;

namespace NutriTec.Application.Abstractions.Services;

/*
 * Descripción:
 * Define las reglas de aplicación para que un nutricionista busque clientes y los
 * asocie como pacientes.
 *
 * Entradas:
 * Recibe DTOs de solicitud provenientes de la API.
 *
 * Salidas:
 * Devuelve DTOs preparados para la respuesta de la API.
 *
 * Restricciones:
 * No expone entidades de dominio fuera de la capa de aplicación.
 */

public interface IPacienteService
{
    /*
     * Descripción: Busca clientes candidatos a ser pacientes.
     * Entradas: Criterio de búsqueda y cancelación.
     * Salidas: Colección de clientes coincidentes.
     * Restricciones: El criterio no puede estar vacío.
     */

    Task<IReadOnlyCollection<ClienteBusquedaResponse>> BuscarClientesAsync(string criterio, CancellationToken cancellationToken);

    /*
     * Descripción: Asocia un cliente como paciente de un nutricionista.
     * Entradas: Identificador del nutricionista, DTO con el cliente y cancelación.
     * Salidas: Devuelve la asociación creada como DTO.
     * Restricciones: El cliente debe existir y no puede estar ya asociado al mismo nutricionista.
     */

    Task<PacienteNutricionistaResponse> AsociarAsync(Guid idNutricionista, AsociarPacienteRequest request, CancellationToken cancellationToken);

    /*
     * Descripción: Lista los pacientes asociados a un nutricionista.
     * Entradas: Identificador del nutricionista y cancelación.
     * Salidas: Colección de asociaciones como DTOs.
     * Restricciones: El identificador no puede ser vacío.
     */

    Task<IReadOnlyCollection<PacienteNutricionistaResponse>> ListarPorNutricionistaAsync(Guid idNutricionista, CancellationToken cancellationToken);

    /*
     * Descripción: Elimina la asociación entre un nutricionista y un paciente.
     * Entradas: Identificador de la asociación y cancelación.
     * Salidas: Devuelve verdadero si la asociación existía y fue eliminada.
     * Restricciones: El identificador no puede ser vacío.
     */

    Task<bool> DesasociarAsync(Guid id, CancellationToken cancellationToken);
}
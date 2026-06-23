using NutriTec.Domain.Pacientes;

namespace NutriTec.Application.Abstractions.Persistence;

/*
 * Descripción:
 * Define las operaciones de consulta de usuarios requeridas por el módulo de pacientes
 * para que un nutricionista pueda buscar clientes candidatos a asociar.
 *
 * Entradas:
 * Recibe criterios de búsqueda y cancelación.
 *
 * Salidas:
 * Devuelve resúmenes de usuarios de tipo Cliente.
 *
 * Restricciones:
 * Es de solo lectura; no modifica datos de autenticación ni de credenciales.
 */

public interface IUsuarioConsultaRepository
{
    /*
     * Descripción: Busca clientes cuyo nombre, apellidos o correo contienen el criterio indicado.
     * Entradas: Criterio de búsqueda y cancelación.
     * Salidas: Colección de clientes coincidentes.
     * Restricciones: Solo devuelve usuarios de la tabla USUARIO.
     */

    Task<IReadOnlyCollection<ClienteResumen>> BuscarClientesAsync(string criterio, CancellationToken cancellationToken);

    /*
     * Descripción: Verifica que un identificador corresponda a un usuario existente.
     * Entradas: Identificador del usuario y cancelación.
     * Salidas: Verdadero si el usuario existe.
     * Restricciones: No modifica datos.
     */

    Task<bool> EsClienteAsync(int idUsuario, CancellationToken cancellationToken);
}
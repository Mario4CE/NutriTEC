using Microsoft.EntityFrameworkCore;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Domain.Pacientes;
using NutriTec.Infrastructure.Sql.Persistence;

namespace NutriTec.Infrastructure.Sql.Repositories;

/*
 * Descripción:
 * Implementa la consulta de solo lectura de usuarios para que un nutricionista pueda
 * buscarlos y asociarlos como pacientes.
 *
 * Entradas:
 * Recibe NutriTecDbContext y criterios de búsqueda definidos por Application.
 *
 * Salidas:
 * Devuelve resúmenes de usuarios desde la tabla USUARIO.
 *
 * Restricciones:
 * Es de solo lectura; no modifica datos de autenticación.
 */
public sealed class UsuarioConsultaSqlRepository(NutriTecDbContext context) : IUsuarioConsultaRepository
{
    /*
     * Descripción: Busca usuarios cuyo nombre, apellidos o correo contienen el criterio indicado.
     * Entradas: Criterio de búsqueda y token de cancelación.
     * Salidas: Colección de usuarios coincidentes.
     * Restricciones: No modifica datos.
     */

    public async Task<IReadOnlyCollection<ClienteResumen>> BuscarClientesAsync(string criterio, CancellationToken cancellationToken)
    {
        var usuarios = await context.Usuarios
            .AsNoTracking()
            .Where(usuario =>
                usuario.Nombre.Contains(criterio) ||
                usuario.Apellidos.Contains(criterio) ||
                usuario.Email.Contains(criterio))
            .OrderBy(usuario => usuario.Nombre)
            .ToListAsync(cancellationToken);

        return usuarios.Select(usuario => new ClienteResumen
        {
            Id = usuario.IdUsuario,
            Nombre = usuario.Nombre,
            Apellidos = usuario.Apellidos,
            Correo = usuario.Email
        }).ToArray();
    }

    /*
     * Descripción: Verifica que un identificador corresponda a un usuario existente.
     * Entradas: Identificador del usuario y token de cancelación.
     * Salidas: Verdadero si el usuario existe.
     * Restricciones: No modifica datos.
     */

    public Task<bool> EsClienteAsync(int idUsuario, CancellationToken cancellationToken)
    {
        return context.Usuarios.AnyAsync(usuario => usuario.IdUsuario == idUsuario, cancellationToken);
    }
}
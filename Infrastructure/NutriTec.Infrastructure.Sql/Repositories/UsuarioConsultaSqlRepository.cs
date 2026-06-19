using Microsoft.EntityFrameworkCore;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Domain.Pacientes;
using NutriTec.Infrastructure.Sql.Persistence;

namespace NutriTec.Infrastructure.Sql.Repositories;

/*
 * Descripción:
 * Implementa la consulta de solo lectura de usuarios de tipo Cliente para que un
 * nutricionista pueda buscarlos y asociarlos como pacientes.
 *
 * Entradas:
 * Recibe NutriTecDbContext y criterios de búsqueda definidos por Application.
 *
 * Salidas:
 * Devuelve resúmenes de clientes desde la tabla USUARIO.
 *
 * Restricciones:
 * Es de solo lectura; no modifica datos de autenticación.
 */
public sealed class UsuarioConsultaSqlRepository(NutriTecDbContext context) : IUsuarioConsultaRepository
{
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
            Id = usuario.IdCliente,
            Nombre = usuario.Nombre,
            Apellidos = usuario.Apellidos,
            Correo = usuario.Email
        }).ToArray();
    }

    public Task<bool> EsClienteAsync(Guid idUsuario, CancellationToken cancellationToken)
    {
        return context.Usuarios.AnyAsync(usuario => usuario.IdCliente == idUsuario, cancellationToken);
    }
}
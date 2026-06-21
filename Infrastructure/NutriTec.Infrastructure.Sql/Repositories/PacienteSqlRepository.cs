using Microsoft.EntityFrameworkCore;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Domain.Pacientes;
using NutriTec.Infrastructure.Sql.Persistence;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Repositories;

/*
 * Descripción:
 * Implementa la persistencia relacional de la asociación entre nutricionistas y pacientes
 * mediante Entity Framework Core y SQL Server.
 *
 * Entradas:
 * Recibe NutriTecDbContext y criterios definidos por Application.
 *
 * Salidas:
 * Inserta, consulta y elimina asociaciones entre nutricionistas y pacientes.
 *
 * Restricciones:
 * No contiene reglas de negocio; mapea entre la entidad de dominio y la entidad SQL.
 * La tabla PACIENTE_NUTRICIONISTA usa llave compuesta (cedula_nutricionista, id_usuario),
 * por lo que el Id de dominio (Guid) no se persiste; se genera solo para uso en memoria.
 */
public sealed class PacienteSqlRepository(NutriTecDbContext context) : IPacienteRepository
{
    /*
     * Descripción: Persiste la asociación entre un nutricionista y un paciente.
     * Entradas: Agregado y token de cancelación.
     * Salidas: Agregado persistido.
     * Restricciones: Recibe datos validados.
     */

    public async Task<PacienteNutricionista> AsociarAsync(PacienteNutricionista asociacion, CancellationToken cancellationToken)
    {
        var entidad = new PacienteNutricionistaSql
        {
            CedulaNutricionista = asociacion.IdNutricionista,
            IdUsuario = asociacion.IdPaciente
        };

        context.PacientesNutricionista.Add(entidad);
        await context.SaveChangesAsync(cancellationToken);
        return asociacion;
    }

    /*
     * Descripción: Verifica si un cliente ya está asociado a un nutricionista.
     * Entradas: Cédula del nutricionista, identificador del paciente y token de cancelación.
     * Salidas: Existencia de la asociación.
     * Restricciones: No modifica datos.
     */

    public Task<bool> ExisteAsociacionAsync(string idNutricionista, int idPaciente, CancellationToken cancellationToken)
    {
        return context.PacientesNutricionista.AnyAsync(
            asociacion => asociacion.CedulaNutricionista == idNutricionista && asociacion.IdUsuario == idPaciente,
            cancellationToken);
    }

    /*
     * Descripción: Lista los pacientes asociados a un nutricionista.
     * Entradas: Cédula del nutricionista y token de cancelación.
     * Salidas: Colección de asociaciones.
     * Restricciones: No modifica datos.
     */

    public async Task<IReadOnlyCollection<PacienteNutricionista>> ListarPorNutricionistaAsync(string idNutricionista, CancellationToken cancellationToken)
    {
        var entidades = await context.PacientesNutricionista
            .AsNoTracking()
            .Where(asociacion => asociacion.CedulaNutricionista == idNutricionista)
            .ToListAsync(cancellationToken);

        return entidades.Select(MapearADominio).ToArray();
    }

    /*
     * Descripción: Consulta una asociación por identificador.
     * Entradas: Identificador y token de cancelación.
     * Salidas: Asociación o nulo.
     * Restricciones: La tabla no tiene un identificador único propio; esta operación no
     * está soportada con el esquema actual y siempre devuelve nulo.
     */

    public Task<PacienteNutricionista?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult<PacienteNutricionista?>(null);
    }

    /*
     * Descripción: Verifica si un paciente pertenece a un nutricionista específico.
     * Entradas: Cédula del nutricionista, identificador del paciente y token de cancelación.
     * Salidas: Verdadero si el paciente está asociado a ese nutricionista.
     * Restricciones: No modifica datos.
     */

    public Task<bool> EsPacienteDeAsync(string idNutricionista, int idPaciente, CancellationToken cancellationToken)
    {
        return ExisteAsociacionAsync(idNutricionista, idPaciente, cancellationToken);
    }

    /*
     * Descripción: Elimina la asociación entre un nutricionista y un paciente.
     * Entradas: Identificador de la asociación y token de cancelación.
     * Salidas: Confirmación.
     * Restricciones: No soportado con el esquema actual sin identificador único; devuelve falso.
     */

    public Task<bool> DesasociarAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(false);
    }

    private static PacienteNutricionista MapearADominio(PacienteNutricionistaSql entidad) => new()
    {
        IdNutricionista = entidad.CedulaNutricionista,
        IdPaciente = entidad.IdUsuario,
        FechaAsociacionUtc = DateTime.UtcNow
    };
}
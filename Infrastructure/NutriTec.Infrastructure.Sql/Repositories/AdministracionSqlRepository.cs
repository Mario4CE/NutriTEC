using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Contracts.Administracion;
using NutriTec.Infrastructure.Sql.Persistence;

namespace NutriTec.Infrastructure.Sql.Repositories;

public sealed class AdministracionSqlRepository(NutriTecDbContext context) : IAdministracionRepository
{
    public async Task<IReadOnlyCollection<ReporteCobroNutricionistaResponse>> GenerarReporteCobroAsync(
        decimal montoBasePorPaciente,
        bool incluirSinPacientes,
        CancellationToken cancellationToken)
    {
        var connection = context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.sp_ReporteCobroNutricionistas";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@montoBasePorPaciente", SqlDbType.Decimal)
        {
            Precision = 10,
            Scale = 2,
            Value = montoBasePorPaciente
        });
        command.Parameters.Add(new SqlParameter("@incluirSinPacientes", SqlDbType.Bit)
        {
            Value = incluirSinPacientes
        });

        var reporte = new List<ReporteCobroNutricionistaResponse>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            reporte.Add(new ReporteCobroNutricionistaResponse(
                reader.GetString(reader.GetOrdinal("cedula_nutricionista")),
                reader.GetString(reader.GetOrdinal("nombre_nutricionista")),
                reader.GetString(reader.GetOrdinal("tipo_cobro")),
                reader.GetInt32(reader.GetOrdinal("cantidad_pacientes")),
                reader.GetDecimal(reader.GetOrdinal("monto_base_por_paciente")),
                reader.GetDecimal(reader.GetOrdinal("subtotal")),
                reader.GetDecimal(reader.GetOrdinal("porcentaje_descuento")),
                reader.GetDecimal(reader.GetOrdinal("monto_descuento")),
                reader.GetDecimal(reader.GetOrdinal("total_cobrar"))));
        }

        return reporte;
    }

    public async Task<decimal?> CalcularImcAsync(decimal pesoKg, decimal estaturaCm, CancellationToken cancellationToken)
    {
        return await context.Database
            .SqlQuery<decimal?>($"SELECT dbo.fn_CalcularImc({pesoKg}, {estaturaCm}) AS Value")
            .SingleAsync(cancellationToken);
    }
}

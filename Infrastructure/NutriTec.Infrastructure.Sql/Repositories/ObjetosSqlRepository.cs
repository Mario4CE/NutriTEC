using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Abstractions.Persistence.ObjetosSql;
using NutriTec.Infrastructure.Sql.Persistence;

namespace NutriTec.Infrastructure.Sql.Repositories;

public sealed class ObjetosSqlRepository(NutriTecDbContext dbContext) : IObjetosSqlRepository
{
    public async Task<IReadOnlyCollection<ReporteCobroNutricionistaSql>> ObtenerReporteCobroNutricionistasAsync(
        decimal montoBasePorPaciente,
        bool incluirSinPacientes,
        CancellationToken cancellationToken)
    {
        await using var command = CrearComando("dbo.sp_ReporteCobroNutricionistas", CommandType.StoredProcedure);
        AgregarParametro(command, "@montoBasePorPaciente", montoBasePorPaciente);
        AgregarParametro(command, "@incluirSinPacientes", incluirSinPacientes);

        await AbrirConexionAsync(command, cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var reporte = new List<ReporteCobroNutricionistaSql>();
        while (await reader.ReadAsync(cancellationToken))
        {
            reporte.Add(new ReporteCobroNutricionistaSql(
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

    public async Task<ProductoAprobadoSql?> AprobarProductoConProcedimientoAsync(Guid idProducto, CancellationToken cancellationToken)
    {
        await using var command = CrearComando("dbo.sp_AprobarProducto", CommandType.StoredProcedure);
        AgregarParametro(command, "@idProducto", idProducto);

        await AbrirConexionAsync(command, cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new ProductoAprobadoSql(
            reader.GetGuid(reader.GetOrdinal("id_producto")),
            reader.GetString(reader.GetOrdinal("nombre")),
            reader.GetString(reader.GetOrdinal("codigo_barras")),
            reader.GetDecimal(reader.GetOrdinal("calorias")),
            reader.GetDecimal(reader.GetOrdinal("proteinas")),
            reader.GetDecimal(reader.GetOrdinal("carbohidratos")),
            reader.GetDecimal(reader.GetOrdinal("grasas")),
            reader.GetBoolean(reader.GetOrdinal("aprobado")),
            reader.GetDateTime(reader.GetOrdinal("fecha_creacion_utc")));
    }

    public async Task<int> AsignarPlanPacienteAsync(
        int idPlan,
        int idUsuario,
        DateOnly fechaInicio,
        DateOnly fechaFin,
        CancellationToken cancellationToken)
    {
        await using var command = CrearComando("dbo.sp_AsignarPlanPaciente", CommandType.StoredProcedure);
        AgregarParametro(command, "@idPlan", idPlan);
        AgregarParametro(command, "@idUsuario", idUsuario);
        AgregarParametro(command, "@fechaInicio", fechaInicio.ToDateTime(TimeOnly.MinValue));
        AgregarParametro(command, "@fechaFin", fechaFin.ToDateTime(TimeOnly.MinValue));

        await AbrirConexionAsync(command, cancellationToken);
        var resultado = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(resultado, System.Globalization.CultureInfo.InvariantCulture);
    }

    public async Task<MedidaUsuarioSql> RegistrarMedidaUsuarioAsync(
        int idUsuario,
        DateOnly fecha,
        decimal pesoKg,
        decimal estaturaCm,
        decimal? cintura,
        decimal? cuello,
        decimal? caderas,
        decimal? pctMusculo,
        decimal? pctGrasa,
        CancellationToken cancellationToken)
    {
        await using var command = CrearComando("dbo.sp_RegistrarMedidaUsuario", CommandType.StoredProcedure);
        AgregarParametro(command, "@idUsuario", idUsuario);
        AgregarParametro(command, "@fecha", fecha.ToDateTime(TimeOnly.MinValue));
        AgregarParametro(command, "@pesoKg", pesoKg);
        AgregarParametro(command, "@estaturaCm", estaturaCm);
        AgregarParametro(command, "@cintura", cintura);
        AgregarParametro(command, "@cuello", cuello);
        AgregarParametro(command, "@caderas", caderas);
        AgregarParametro(command, "@pctMusculo", pctMusculo);
        AgregarParametro(command, "@pctGrasa", pctGrasa);

        await AbrirConexionAsync(command, cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            throw new InvalidOperationException("El procedimiento no devolvió la medida registrada.");
        }

        return new MedidaUsuarioSql(
            reader.GetInt32(reader.GetOrdinal("id_medida")),
            reader.GetInt32(reader.GetOrdinal("id_usuario")),
            DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("fecha"))),
            reader.GetDecimal(reader.GetOrdinal("peso")),
            reader.GetDecimal(reader.GetOrdinal("imc")));
    }

    public async Task<decimal?> CalcularImcAsync(decimal pesoKg, decimal estaturaCm, CancellationToken cancellationToken)
    {
        await using var command = CrearComando("SELECT dbo.fn_CalcularImc(@pesoKg, @estaturaCm);", CommandType.Text);
        AgregarParametro(command, "@pesoKg", pesoKg);
        AgregarParametro(command, "@estaturaCm", estaturaCm);

        await AbrirConexionAsync(command, cancellationToken);
        var resultado = await command.ExecuteScalarAsync(cancellationToken);
        return resultado is null or DBNull ? null : Convert.ToDecimal(resultado, System.Globalization.CultureInfo.InvariantCulture);
    }

    public async Task<decimal> ObtenerTotalCaloriasPlanAsync(int idPlan, CancellationToken cancellationToken)
    {
        await using var command = CrearComando("SELECT dbo.fn_TotalCaloriasPlan(@idPlan);", CommandType.Text);
        AgregarParametro(command, "@idPlan", idPlan);

        await AbrirConexionAsync(command, cancellationToken);
        var resultado = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToDecimal(resultado, System.Globalization.CultureInfo.InvariantCulture);
    }

    private DbCommand CrearComando(string texto, CommandType commandType)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = texto;
        command.CommandType = commandType;
        return command;
    }

    private async Task AbrirConexionAsync(DbCommand command, CancellationToken cancellationToken)
    {
        if (command.Connection is null)
        {
            throw new InvalidOperationException("No hay conexión SQL configurada para ejecutar el objeto programable.");
        }

        if (command.Connection.State != ConnectionState.Open)
        {
            await command.Connection.OpenAsync(cancellationToken);
        }
    }

    private static void AgregarParametro(DbCommand command, string nombre, object? valor)
    {
        var parametro = command.CreateParameter();
        parametro.ParameterName = nombre;
        parametro.Value = valor ?? DBNull.Value;
        command.Parameters.Add(parametro);
    }
}

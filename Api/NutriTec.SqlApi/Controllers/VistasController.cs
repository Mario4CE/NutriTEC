using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NutriTec.Contracts.Responses;
using NutriTec.Infrastructure.Sql.Persistence;

namespace NutriTec.SqlApi.Controllers;

[ApiController]
[Authorize]
public sealed class VistasController(NutriTecDbContext context) : ControllerBase
{
    private async Task<SqlConnection> OpenAsync(CancellationToken ct)
    {
        var connection = (SqlConnection)context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open) await connection.OpenAsync(ct);
        return connection;
    }

    [HttpGet("api/planes/usuario/{idUsuario:int}")]
    [Authorize(Policy = "Cliente")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<object>>>> PlanesUsuario(int idUsuario, CancellationToken ct)
    {
        const string sql = """
            SELECT a.id_asignacion, a.id_plan, p.nombre, p.cedula_nutricionista, p.total_calorias, a.fecha_inicio, a.fecha_fin
            FROM ASIGNACION_PLAN a INNER JOIN PLAN_ALIMENTACION p ON p.id_plan = a.id_plan
            WHERE a.id_usuario = @idUsuario ORDER BY a.fecha_inicio DESC
            """;
        return Ok(ApiResponse<IReadOnlyCollection<object>>.SuccessResponse(await QueryAsync(sql, ct, P("@idUsuario", idUsuario))));
    }

    [HttpPost("api/registros-diarios")]
    [Authorize(Policy = "Cliente")]
    public async Task<ActionResult<ApiResponse<object>>> CrearRegistroDiario([FromBody] RegistroDiarioRequest r, CancellationToken ct)
    {
        var cn = await OpenAsync(ct);
        await using var tx = (SqlTransaction)await cn.BeginTransactionAsync(ct);
        var id = await ScalarAsync<int>(cn, tx, "INSERT INTO REGISTRO_DIARIO (id_usuario, fecha, tipo_comida) OUTPUT INSERTED.id_registro VALUES (@u,@f,@t)", ct, P("@u", r.IdUsuario), P("@f", r.Fecha), P("@t", r.TipoComida));
        foreach (var p in r.Productos) await NonQueryAsync(cn, tx, "INSERT INTO REGISTRO_PRODUCTO (id_registro,id_producto,cantidad_porciones) VALUES (@r,@p,@c)", ct, P("@r", id), P("@p", p.IdProducto), P("@c", p.CantidadPorciones));
        await tx.CommitAsync(ct);
        return StatusCode(201, ApiResponse<object>.SuccessResponse(new { IdRegistro = id }, "Registro diario creado."));
    }

    [HttpGet("api/registros-diarios/usuario/{idUsuario:int}")]
    [Authorize(Policy = "Cliente")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<object>>>> RegistrosUsuario(int idUsuario, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyCollection<object>>.SuccessResponse(await QueryAsync("SELECT id_registro, id_usuario, fecha, tipo_comida FROM REGISTRO_DIARIO WHERE id_usuario=@idUsuario ORDER BY fecha DESC, id_registro DESC", ct, P("@idUsuario", idUsuario))));

    [HttpPost("api/recetas")]
    [Authorize(Policy = "Cliente")]
    public async Task<ActionResult<ApiResponse<object>>> CrearReceta([FromBody] RecetaRequest r, CancellationToken ct)
    {
        var cn = await OpenAsync(ct); await using var tx = (SqlTransaction)await cn.BeginTransactionAsync(ct);
        var id = await ScalarAsync<int>(cn, tx, "INSERT INTO RECETA (nombre,id_usuario) OUTPUT INSERTED.id_receta VALUES (@n,@u)", ct, P("@n", r.Nombre), P("@u", r.IdUsuario));
        foreach (var p in r.Productos) await NonQueryAsync(cn, tx, "INSERT INTO RECETA_PRODUCTO (id_receta,id_producto,cantidad_porciones) VALUES (@r,@p,@c)", ct, P("@r", id), P("@p", p.IdProducto), P("@c", p.CantidadPorciones));
        await tx.CommitAsync(ct); return StatusCode(201, ApiResponse<object>.SuccessResponse(new { IdReceta = id }, "Receta creada."));
    }

    [HttpGet("api/recetas/usuario/{idUsuario:int}")]
    [Authorize(Policy = "Cliente")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<object>>>> RecetasUsuario(int idUsuario, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyCollection<object>>.SuccessResponse(await QueryAsync("SELECT id_receta, nombre, id_usuario, total_calorias, total_carbohidratos, total_proteina, total_grasa FROM RECETA WHERE id_usuario=@idUsuario ORDER BY id_receta DESC", ct, P("@idUsuario", idUsuario))));

    [HttpGet("api/medidas/usuario/{idUsuario:int}")]
    [Authorize(Policy = "Cliente")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<object>>>> MedidasUsuario(int idUsuario, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyCollection<object>>.SuccessResponse(await QueryAsync("SELECT id_medida,id_usuario,fecha,cintura,cuello,caderas,pct_musculo,pct_grasa FROM MEDIDA_USUARIO WHERE id_usuario=@idUsuario ORDER BY fecha DESC,id_medida DESC", ct, P("@idUsuario", idUsuario))));

    [HttpPost("api/medidas/usuario/{idUsuario:int}")]
    [Authorize(Policy = "Cliente")]
    public async Task<ActionResult<ApiResponse<object>>> RegistrarMedidaCliente(int idUsuario, [FromBody] MedidaRequest r, CancellationToken ct)
    {
        var id = await ScalarAsync<int>(await OpenAsync(ct), null, "INSERT INTO MEDIDA_USUARIO (id_usuario,fecha,cintura,cuello,caderas,pct_musculo,pct_grasa) OUTPUT INSERTED.id_medida VALUES (@u,@f,@ci,@cu,@ca,@pm,@pg)", ct, P("@u", idUsuario), P("@f", r.Fecha), P("@ci", r.Cintura), P("@cu", r.Cuello), P("@ca", r.Caderas), P("@pm", r.PctMusculo), P("@pg", r.PctGrasa));
        return StatusCode(201, ApiResponse<object>.SuccessResponse(new { IdMedida = id }, "Medida registrada."));
    }

    [HttpGet("api/pacientes/nutricionista/{cedula}")]
    [Authorize(Policy = "Nutricionista")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<object>>>> Pacientes(string cedula, CancellationToken ct) => Ok(ApiResponse<IReadOnlyCollection<object>>.SuccessResponse(await QueryAsync("SELECT u.id_usuario,u.nombre,u.apellidos,u.email,u.peso,u.imc FROM PACIENTE_NUTRICIONISTA pn INNER JOIN USUARIO u ON u.id_usuario=pn.id_usuario WHERE pn.cedula_nutricionista=@c ORDER BY u.nombre", ct, P("@c", cedula))));

    [HttpPost("api/pacientes/asociar")]
    [Authorize(Policy = "Nutricionista")]
    public async Task<ActionResult<ApiResponse<object>>> Asociar([FromBody] AsociarPacienteRequest r, CancellationToken ct)
    { await NonQueryAsync(await OpenAsync(ct), null, "INSERT INTO PACIENTE_NUTRICIONISTA (cedula_nutricionista,id_usuario) VALUES (@c,@u)", ct, P("@c", r.CedulaNutricionista), P("@u", r.IdUsuario)); return StatusCode(201, ApiResponse<object>.SuccessResponse(r, "Paciente asociado.")); }

    [HttpGet("api/planes/nutricionista/{cedula}")]
    [Authorize(Policy = "Nutricionista")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<object>>>> PlanesNutricionista(string cedula, CancellationToken ct) => Ok(ApiResponse<IReadOnlyCollection<object>>.SuccessResponse(await QueryAsync("SELECT id_plan,nombre,cedula_nutricionista,total_calorias FROM PLAN_ALIMENTACION WHERE cedula_nutricionista=@c ORDER BY nombre", ct, P("@c", cedula))));

    [HttpPost("api/vistas/planes")]
    [Authorize(Policy = "Nutricionista")]
    public async Task<ActionResult<ApiResponse<object>>> CrearPlan([FromBody] PlanRequest r, CancellationToken ct)
    { var id = await ScalarAsync<int>(await OpenAsync(ct), null, "INSERT INTO PLAN_ALIMENTACION (nombre,cedula_nutricionista) OUTPUT INSERTED.id_plan VALUES (@n,@c)", ct, P("@n", r.Nombre), P("@c", r.CedulaNutricionista)); return StatusCode(201, ApiResponse<object>.SuccessResponse(new { IdPlan = id }, "Plan creado.")); }

    [HttpPost("api/planes/{idPlan:int}/tiempos-comida")]
    [Authorize(Policy = "Nutricionista")]
    public async Task<ActionResult<ApiResponse<object>>> AgregarTiempo(int idPlan, [FromBody] TiempoComidaRequest r, CancellationToken ct)
    {
        var cn = await OpenAsync(ct); await using var tx = (SqlTransaction)await cn.BeginTransactionAsync(ct);
        var id = await ScalarAsync<int>(cn, tx, "INSERT INTO TIEMPO_COMIDA_PLAN (id_plan,tipo_comida) OUTPUT INSERTED.id_tiempo VALUES (@p,@t)", ct, P("@p", idPlan), P("@t", r.TipoComida));
        foreach (var p in r.Productos) await NonQueryAsync(cn, tx, "INSERT INTO PLAN_PRODUCTO (id_tiempo,id_producto,cantidad_porciones) VALUES (@t,@p,@c)", ct, P("@t", id), P("@p", p.IdProducto), P("@c", p.CantidadPorciones));
        await tx.CommitAsync(ct); return StatusCode(201, ApiResponse<object>.SuccessResponse(new { IdTiempo = id }, "Tiempo de comida agregado."));
    }

    [HttpGet("api/registro-diario/paciente/{idUsuario:int}")]
    [Authorize(Policy = "Nutricionista")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<object>>>> RegistroPaciente(int idUsuario, CancellationToken ct) => await RegistrosUsuario(idUsuario, ct);

    private static SqlParameter P(string name, object? value) => new(name, value switch
    {
        null => DBNull.Value,
        DateOnly date => date.ToDateTime(TimeOnly.MinValue),
        _ => value
    });

    private async Task<IReadOnlyCollection<object>> QueryAsync(string sql, CancellationToken ct, params SqlParameter[] ps)
    {
        var cn = await OpenAsync(ct); await using var cmd = new SqlCommand(sql, cn); cmd.Parameters.AddRange(ps); await using var rd = await cmd.ExecuteReaderAsync(ct);
        var rows = new List<object>(); while (await rd.ReadAsync(ct)) { var d = new Dictionary<string, object?>(); for (var i = 0; i < rd.FieldCount; i++) d[rd.GetName(i)] = rd.IsDBNull(i) ? null : rd.GetValue(i); rows.Add(d); } return rows;
    }
    private static async Task<T> ScalarAsync<T>(SqlConnection cn, SqlTransaction? tx, string sql, CancellationToken ct, params SqlParameter[] ps) { await using var cmd = new SqlCommand(sql, cn, tx); cmd.Parameters.AddRange(ps); return (T)await cmd.ExecuteScalarAsync(ct)!; }
    private static async Task NonQueryAsync(SqlConnection cn, SqlTransaction? tx, string sql, CancellationToken ct, params SqlParameter[] ps) { await using var cmd = new SqlCommand(sql, cn, tx); cmd.Parameters.AddRange(ps); await cmd.ExecuteNonQueryAsync(ct); }
}

public sealed record ProductoCantidadRequest(Guid IdProducto, decimal CantidadPorciones);
public sealed record RegistroDiarioRequest(int IdUsuario, DateOnly Fecha, string TipoComida, IReadOnlyCollection<ProductoCantidadRequest> Productos);
public sealed record RecetaRequest(int IdUsuario, string Nombre, IReadOnlyCollection<ProductoCantidadRequest> Productos);
public sealed record MedidaRequest(DateOnly Fecha, decimal? Cintura, decimal? Cuello, decimal? Caderas, decimal? PctMusculo, decimal? PctGrasa);
public sealed record AsociarPacienteRequest(string CedulaNutricionista, int IdUsuario);
public sealed record PlanRequest(string Nombre, string CedulaNutricionista);
public sealed record TiempoComidaRequest(string TipoComida, IReadOnlyCollection<ProductoCantidadRequest> Productos);
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Contracts.ObjetosSql;

namespace NutriTec.Application.ObjetosSql;

public sealed class ObjetosSqlService(IObjetosSqlRepository repository) : IObjetosSqlService
{
    public async Task<IReadOnlyCollection<ReporteCobroNutricionistaResponse>> ObtenerReporteCobroNutricionistasAsync(
        decimal montoBasePorPaciente,
        bool incluirSinPacientes,
        CancellationToken cancellationToken)
    {
        if (montoBasePorPaciente <= 0)
        {
            throw new ArgumentException("El monto base por paciente debe ser mayor que cero.", nameof(montoBasePorPaciente));
        }

        var reporte = await repository.ObtenerReporteCobroNutricionistasAsync(montoBasePorPaciente, incluirSinPacientes, cancellationToken);
        return reporte.Select(fila => new ReporteCobroNutricionistaResponse(
            fila.CedulaNutricionista,
            fila.NombreNutricionista,
            fila.TipoCobro,
            fila.CantidadPacientes,
            fila.MontoBasePorPaciente,
            fila.Subtotal,
            fila.PorcentajeDescuento,
            fila.MontoDescuento,
            fila.TotalCobrar)).ToArray();
    }

    public async Task<ProductoAprobadoSqlResponse?> AprobarProductoConProcedimientoAsync(Guid idProducto, CancellationToken cancellationToken)
    {
        if (idProducto == Guid.Empty)
        {
            throw new ArgumentException("El identificador del producto no puede estar vacío.", nameof(idProducto));
        }

        var producto = await repository.AprobarProductoConProcedimientoAsync(idProducto, cancellationToken);
        return producto is null
            ? null
            : new ProductoAprobadoSqlResponse(
                producto.IdProducto,
                producto.Nombre,
                producto.CodigoBarras,
                producto.Calorias,
                producto.Proteinas,
                producto.Carbohidratos,
                producto.Grasas,
                producto.Aprobado,
                producto.FechaCreacionUtc);
    }

    public async Task<AsignarPlanPacienteResponse> AsignarPlanPacienteAsync(
        int idPlan,
        int idUsuario,
        AsignarPlanPacienteRequest request,
        CancellationToken cancellationToken)
    {
        ValidarEnteroPositivo(idPlan, nameof(idPlan));
        ValidarEnteroPositivo(idUsuario, nameof(idUsuario));

        if (request.FechaFin < request.FechaInicio)
        {
            throw new ArgumentException("La fecha final no puede ser menor a la fecha inicial.", nameof(request));
        }

        var idAsignacion = await repository.AsignarPlanPacienteAsync(idPlan, idUsuario, request.FechaInicio, request.FechaFin, cancellationToken);
        return new AsignarPlanPacienteResponse(idAsignacion);
    }

    public async Task<RegistrarMedidaUsuarioResponse> RegistrarMedidaUsuarioAsync(
        int idUsuario,
        RegistrarMedidaUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        ValidarEnteroPositivo(idUsuario, nameof(idUsuario));
        ValidarDecimalPositivo(request.PesoKg, nameof(request.PesoKg));
        ValidarDecimalPositivo(request.EstaturaCm, nameof(request.EstaturaCm));

        var medida = await repository.RegistrarMedidaUsuarioAsync(
            idUsuario,
            request.Fecha,
            request.PesoKg,
            request.EstaturaCm,
            request.Cintura,
            request.Cuello,
            request.Caderas,
            request.PctMusculo,
            request.PctGrasa,
            cancellationToken);

        return new RegistrarMedidaUsuarioResponse(medida.IdMedida, medida.IdUsuario, medida.Fecha, medida.Peso, medida.Imc);
    }

    public async Task<CalcularImcResponse> CalcularImcAsync(decimal pesoKg, decimal estaturaCm, CancellationToken cancellationToken)
    {
        ValidarDecimalPositivo(pesoKg, nameof(pesoKg));
        ValidarDecimalPositivo(estaturaCm, nameof(estaturaCm));

        var imc = await repository.CalcularImcAsync(pesoKg, estaturaCm, cancellationToken);
        return new CalcularImcResponse(pesoKg, estaturaCm, imc);
    }

    public async Task<TotalCaloriasPlanResponse> ObtenerTotalCaloriasPlanAsync(int idPlan, CancellationToken cancellationToken)
    {
        ValidarEnteroPositivo(idPlan, nameof(idPlan));
        var total = await repository.ObtenerTotalCaloriasPlanAsync(idPlan, cancellationToken);
        return new TotalCaloriasPlanResponse(idPlan, total);
    }

    private static void ValidarEnteroPositivo(int valor, string nombreParametro)
    {
        if (valor <= 0)
        {
            throw new ArgumentException("El valor debe ser mayor que cero.", nombreParametro);
        }
    }

    private static void ValidarDecimalPositivo(decimal valor, string nombreParametro)
    {
        if (valor <= 0)
        {
            throw new ArgumentException("El valor debe ser mayor que cero.", nombreParametro);
        }
    }
}

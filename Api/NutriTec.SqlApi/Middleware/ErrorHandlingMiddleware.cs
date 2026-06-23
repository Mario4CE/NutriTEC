using System.Runtime.ExceptionServices;
using NutriTec.Application.Common;
using NutriTec.Contracts.Common;

namespace NutriTec.SqlApi.Middleware;

public sealed class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    /*
    Descripción:
    Captura excepciones no manejadas por controllers y las transforma en respuestas HTTP consistentes.

    Entradas:
    HttpContext de la solicitud entrante y excepciones lanzadas por capas posteriores del pipeline.

    Salidas:
    ErrorResponse con código estable y mensaje seguro para el cliente HTTP.

    Restricciones:
    No expone stack traces, detalles de SQL, secretos ni información sensible; no implementa autenticación ni autorización.
    */
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await EscribirRespuestaErrorAsync(context, exception);
        }
    }

    private async Task EscribirRespuestaErrorAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            logger.LogError(exception, "No fue posible escribir la respuesta de error porque la respuesta HTTP ya había iniciado.");
            ExceptionDispatchInfo.Capture(exception).Throw();
        }

        var (statusCode, response) = MapearError(exception);

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Ocurrió un error no controlado al procesar la solicitud.");
        }
        else
        {
            logger.LogWarning(exception, "La solicitud terminó con un error controlado: {CodigoError}.", response.Codigo);
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(response);
    }

    private static (int StatusCode, ErrorResponse Response) MapearError(Exception exception) => exception switch
    {
        ConflictoException conflicto => (
            StatusCodes.Status409Conflict,
            new ErrorResponse("conflicto", conflicto.Message)),

        ArgumentException argumento => (
            StatusCodes.Status400BadRequest,
            new ErrorResponse("solicitud_invalida", argumento.Message)),

        InvalidOperationException => (
            StatusCodes.Status400BadRequest,
            new ErrorResponse("operacion_invalida", "La operación solicitada no es válida.")),

        _ => (
            StatusCodes.Status500InternalServerError,
            new ErrorResponse("error_interno", "Ocurrió un error inesperado."))
    };
}

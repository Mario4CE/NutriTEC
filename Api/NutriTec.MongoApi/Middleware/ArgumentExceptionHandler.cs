using Microsoft.AspNetCore.Diagnostics;
using NutriTec.Contracts.Responses;

namespace NutriTec.MongoApi.Middleware;

/*
 * Descripción:
 * Traduce errores de validación de la capa de aplicación a respuestas HTTP controladas.
 *
 * Entradas:
 * Recibe la excepción propagada y el contexto HTTP actual.
 *
 * Salidas:
 * Escribe una respuesta HTTP 400 estandarizada cuando la excepción corresponde a un argumento inválido.
 *
 * Restricciones:
 * No intercepta excepciones inesperadas; estas continúan hacia el manejador predeterminado del framework.
 */
public sealed class ArgumentExceptionHandler : IExceptionHandler
{
    /*
     * Descripción:
     * Intenta manejar una excepción de validación conocida.
     *
     * Entradas:
     * Recibe contexto HTTP, excepción y token de cancelación.
     *
     * Salidas:
     * Devuelve verdadero si escribió la respuesta o falso si la excepción no corresponde a este manejador.
     *
     * Restricciones:
     * Solo convierte instancias de ArgumentException para no ocultar errores internos.
     */
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ArgumentException argumentException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(
            ApiResponse<object>.ErrorResponse(
                "La solicitud contiene datos inválidos.",
                [argumentException.Message]),
            cancellationToken);

        return true;
    }
}

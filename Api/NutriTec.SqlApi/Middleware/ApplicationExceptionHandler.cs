using Microsoft.AspNetCore.Diagnostics;
using NutriTec.Application.Common;
using NutriTec.Contracts.Responses;

namespace NutriTec.SqlApi.Middleware;

/*
 * Descripción:
 * Traduce errores esperados de Application a respuestas HTTP controladas para la API SQL.
 *
 * Entradas:
 * Recibe la excepción propagada y el contexto HTTP actual.
 *
 * Salidas:
 * Escribe respuestas estandarizadas 400 para validaciones y 409 para conflictos de datos.
 *
 * Restricciones:
 * No intercepta excepciones inesperadas para evitar ocultar errores internos.
 */

public sealed class ApplicationExceptionHandler : IExceptionHandler
{

    /*
     * Descripción:
     * Intenta convertir una excepción conocida en una respuesta HTTP estándar.
     *
     * Entradas:
     * Recibe contexto HTTP, excepción y token de cancelación.
     *
     * Salidas:
     * Devuelve verdadero cuando escribió la respuesta o falso cuando la excepción no corresponde al manejador.
     *
     * Restricciones:
     * Solo maneja ArgumentException y ConflictoException generadas por reglas de aplicación.
     */

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            ConflictoException => StatusCodes.Status409Conflict,
            _ => 0
        };

        if (statusCode == 0)
        {
            return false;
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            ApiResponse<object>.ErrorResponse(exception.Message),
            cancellationToken);

        return true;
    }
}

using Microsoft.AspNetCore.Diagnostics;
using NutriTec.Contracts.Responses;

namespace NutriTec.MongoApi.Middleware;

/*
 * Descripción:
 * Traduce errores de validación de la capa de aplicación a respuestas HTTP controladas.
 * A que se refiere esto? Se refiere a que cuando la capa de aplicación lanza una excepción de validación (como ArgumentException) debido a datos de entrada inválidos, 
 * este manejador captura esa excepción y devuelve una respuesta HTTP 400 con un mensaje de error estandarizado. 
 * Esto ayuda a mantener una interfaz de API consistente y clara para los clientes, en lugar de exponer detalles internos o stack traces.
 * 
 * Que es stack trace? Es la información detallada sobre la secuencia de llamadas a métodos que llevaron a una excepción, útil para depuración pero no adecuada para exponer a los clientes.
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
        if (exception is not ArgumentException argumentException)  // Solo manejamos ArgumentException, otras excepciones se dejan pasar
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(  // Escribimos una respuesta JSON estandarizada para errores de validación
            ApiResponse<object>.ErrorResponse(
                "La solicitud contiene datos inválidos.",
                [argumentException.Message]),
            cancellationToken);

        return true;
    }
}

/*
 * Notas: a que nos referimos con estandarizadas?
 * Nos referimos a que la respuesta JSON sigue un formato consistente definido por ApiResponse<T>, con campos como "success", "message" y "errors".
 */

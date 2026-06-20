namespace NutriTec.SqlApi.Middleware;

public sealed class NoStoreAuthResponseMiddleware(RequestDelegate next)
{
    private const string AuthPathPrefix = "/api/auth";

    /*
    Descripción:
    Agrega headers anti-cache a respuestas de autenticación para evitar almacenar JWT o datos sensibles.

    Entradas:
    HttpContext de solicitudes que pasan por el pipeline HTTP de la API SQL.

    Salidas:
    Respuestas bajo /api/auth con Cache-Control, Pragma y Expires configurados antes de enviarse al cliente.

    Restricciones:
    No modifica el cuerpo de la respuesta, no autentica usuarios, no consulta base de datos y no registra información sensible.
    */
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments(AuthPathPrefix, StringComparison.OrdinalIgnoreCase))
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers["Cache-Control"] = "no-store";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "0";

                return Task.CompletedTask;
            });
        }

        await next(context);
    }
}

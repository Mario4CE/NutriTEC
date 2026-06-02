namespace NutriTec.Contracts.Responses;

/*
 * Descripción:
 * Estandariza la estructura de las respuestas enviadas por las APIs de NutriTEC.
 *
 * Entradas:
 * Recibe datos opcionales, un mensaje descriptivo y una lista opcional de errores.
 *
 * Salidas:
 * Expone un contrato uniforme para respuestas exitosas o fallidas.
 *
 * Restricciones:
 * Los controllers deben devolver DTOs dentro de Data en lugar de exponer entidades del dominio.
 */
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    /*
     * Descripción:
     * Construye una respuesta exitosa con datos.
     *
     * Entradas:
     * Recibe los datos de respuesta y un mensaje opcional.
     *
     * Salidas:
     * Devuelve una respuesta marcada como exitosa.
     *
     * Restricciones:
     * No agrega errores cuando la operación finalizó correctamente.
     */
    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = null
        };
    }

    /*
     * Descripción:
     * Construye una respuesta fallida sin datos.
     *
     * Entradas:
     * Recibe un mensaje y una lista opcional de detalles de error.
     *
     * Salidas:
     * Devuelve una respuesta marcada como fallida.
     *
     * Restricciones:
     * Data siempre se establece con el valor predeterminado del tipo genérico.
     */
    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors
        };
    }
}

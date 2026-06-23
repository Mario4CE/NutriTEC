using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Contracts.Retroalimentaciones;
using NutriTec.Domain.Retroalimentaciones;

namespace NutriTec.Application.Retroalimentaciones;

/*
 * Descripción:
 * Implementa las reglas de aplicación para crear, consultar y responder foros de retroalimentación.
 *
 * Entradas:
 * Recibe solicitudes validadas superficialmente por ASP.NET Core y un repositorio desacoplado.
 *
 * Salidas:
 * Devuelve DTOs preparados para la API o confirma la actualización de un foro existente.
 *
 * Restricciones:
 * Rechaza identificadores vacíos y textos sin contenido; genera fechas UTC desde el servidor.
 */
public sealed class RetroalimentacionService(IRetroalimentacionRepository repository) : IRetroalimentacionService
{
    /*
     * Descripción:
     * Abre un foro con su mensaje inicial.
     *
     * Entradas:
     * Recibe participantes, autor, contenido y token de cancelación.
     *
     * Salidas:
     * Devuelve el foro creado como DTO.
     *
     * Restricciones:
     * Todos los identificadores y textos son obligatorios.
     */
    public async Task<RetroalimentacionResponse> CrearAsync(
        CrearRetroalimentacionRequest request,
        CancellationToken cancellationToken)
    {
        ValidarIdentificador(request.IdPaciente, nameof(request.IdPaciente));
        ValidarIdentificador(request.IdNutricionista, nameof(request.IdNutricionista));
        ValidarTexto(request.Autor, nameof(request.Autor));
        ValidarTexto(request.Mensaje, nameof(request.Mensaje));

        var fechaCreacionUtc = DateTime.UtcNow;
        var retroalimentacion = new Retroalimentacion
        {
            Id = Guid.NewGuid(),
            IdPaciente = request.IdPaciente,
            IdNutricionista = request.IdNutricionista,
            FechaCreacionUtc = fechaCreacionUtc,
            Mensajes =
            [
                new MensajeRetroalimentacion
                {
                    Autor = request.Autor.Trim(),
                    Mensaje = request.Mensaje.Trim(),
                    FechaUtc = fechaCreacionUtc
                }
            ]
        };

        var creada = await repository.CrearAsync(retroalimentacion, cancellationToken);
        return Mapear(creada);
    }

    /*
     * Descripción:
     * Consulta los foros abiertos para un paciente.
     *
     * Entradas:
     * Recibe el identificador del paciente y token de cancelación.
     *
     * Salidas:
     * Devuelve DTOs ordenados por la persistencia.
     *
     * Restricciones:
     * El identificador del paciente no puede ser vacío.
     */
    public async Task<IReadOnlyCollection<RetroalimentacionResponse>> ObtenerPorPacienteAsync(
        Guid idPaciente,
        CancellationToken cancellationToken)
    {
        ValidarIdentificador(idPaciente, nameof(idPaciente));
        var retroalimentaciones = await repository.ObtenerPorPacienteAsync(idPaciente, cancellationToken);
        return retroalimentaciones.Select(Mapear).ToArray();
    }

    /*
     * Descripción:
     * Consulta los foros asociados con un nutricionista.
     *
     * Entradas:
     * Recibe el identificador del nutricionista y token de cancelación.
     *
     * Salidas:
     * Devuelve DTOs ordenados por la persistencia.
     *
     * Restricciones:
     * El identificador del nutricionista no puede ser vacío.
     */
    public async Task<IReadOnlyCollection<RetroalimentacionResponse>> ObtenerPorNutricionistaAsync(
        Guid idNutricionista,
        CancellationToken cancellationToken)
    {
        ValidarIdentificador(idNutricionista, nameof(idNutricionista));
        var retroalimentaciones = await repository.ObtenerPorNutricionistaAsync(idNutricionista, cancellationToken);
        return retroalimentaciones.Select(Mapear).ToArray();
    }

    /*
     * Descripción:
     * Agrega una respuesta a un foro existente.
     *
     * Entradas:
     * Recibe el identificador del foro, autor, contenido y token de cancelación.
     *
     * Salidas:
     * Devuelve verdadero si MongoDB encontró y actualizó el documento.
     *
     * Restricciones:
     * El identificador, autor y contenido son obligatorios.
     */
    public Task<bool> ResponderAsync(
        Guid idRetroalimentacion,
        ResponderRetroalimentacionRequest request,
        CancellationToken cancellationToken)
    {
        ValidarIdentificador(idRetroalimentacion, nameof(idRetroalimentacion));
        ValidarTexto(request.Autor, nameof(request.Autor));
        ValidarTexto(request.Mensaje, nameof(request.Mensaje));

        return repository.AgregarMensajeAsync(
            idRetroalimentacion,
            new MensajeRetroalimentacion
            {
                Autor = request.Autor.Trim(),
                Mensaje = request.Mensaje.Trim(),
                FechaUtc = DateTime.UtcNow
            },
            cancellationToken);
    }

    private static RetroalimentacionResponse Mapear(Retroalimentacion retroalimentacion)
    {
        return new RetroalimentacionResponse(
            retroalimentacion.Id,
            retroalimentacion.IdPaciente,
            retroalimentacion.IdNutricionista,
            retroalimentacion.FechaCreacionUtc,
            retroalimentacion.Mensajes
                .Select(mensaje => new MensajeRetroalimentacionResponse(mensaje.Autor, mensaje.Mensaje, mensaje.FechaUtc))
                .ToArray());
    }

    private static void ValidarIdentificador(Guid identificador, string nombreParametro)
    {
        if (identificador == Guid.Empty)
        {
            throw new ArgumentException("El identificador no puede estar vacío.", nombreParametro);
        }
    }

    private static void ValidarTexto(string texto, string nombreParametro)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            throw new ArgumentException("El texto no puede estar vacío.", nombreParametro);
        }
    }
}

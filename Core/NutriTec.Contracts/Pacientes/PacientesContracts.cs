namespace NutriTec.Contracts.Pacientes;

public sealed record AsociarPacienteRequest(
    int IdCliente);

public sealed record PacienteNutricionistaResponse(
    Guid Id,
    string IdNutricionista,
    int IdPaciente,
    DateTime FechaAsociacionUtc);

public sealed record ClienteBusquedaResponse(
    int Id,
    string Nombre,
    string Apellidos,
    string Correo);
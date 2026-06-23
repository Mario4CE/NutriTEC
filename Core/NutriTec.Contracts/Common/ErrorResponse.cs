namespace NutriTec.Contracts.Common;

public sealed record ErrorResponse(
    string Codigo,
    string Mensaje);

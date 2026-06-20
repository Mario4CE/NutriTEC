namespace NutriTec.Contracts.Administracion;

public sealed record CalculoImcResponse(
    decimal PesoKg,
    decimal EstaturaCm,
    decimal? Imc);

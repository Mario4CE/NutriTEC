namespace NutriTec.Contracts.ObjetosSql;

public sealed record CalcularImcResponse(decimal PesoKg, decimal EstaturaCm, decimal? Imc);

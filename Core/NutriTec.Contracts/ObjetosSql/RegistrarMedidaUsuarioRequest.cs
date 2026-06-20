using System.ComponentModel.DataAnnotations;

namespace NutriTec.Contracts.ObjetosSql;

public sealed class RegistrarMedidaUsuarioRequest
{
    [Required]
    public DateOnly Fecha { get; init; }

    [Range(0.01, 999.99)]
    public decimal PesoKg { get; init; }

    [Range(0.01, 999.99)]
    public decimal EstaturaCm { get; init; }

    public decimal? Cintura { get; init; }
    public decimal? Cuello { get; init; }
    public decimal? Caderas { get; init; }
    public decimal? PctMusculo { get; init; }
    public decimal? PctGrasa { get; init; }
}

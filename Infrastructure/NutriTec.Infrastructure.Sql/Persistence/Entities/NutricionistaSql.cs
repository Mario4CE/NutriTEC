namespace NutriTec.Infrastructure.Sql.Persistence.Entities;

/*
 * Descripción:
 * Representa la tabla NUTRICIONISTA dentro del adaptador SQL Server.
 *
 * Entradas:
 * Recibe identificación, datos personales, datos profesionales, correo y hash de contraseña desde la persistencia relacional.
 *
 * Salidas:
 * Permite a Entity Framework Core materializar nutricionistas sin exponer la entidad al frontend.
 *
 * Restricciones:
 * Es una entidad de infraestructura; la contraseña debe persistirse únicamente como hash y tipo cobro debe respetar los valores aceptados por SQL.
 */
public sealed class NutricionistaSql
{
    public string Cedula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string CodigoNutricionista { get; set; } = string.Empty;
    public int Edad { get; set; }
    public DateOnly FechaNacimiento { get; set; }
    public decimal Peso { get; set; }
    public decimal Imc { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public string TarjetaCredito { get; set; } = string.Empty;
    public string TipoCobro { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}

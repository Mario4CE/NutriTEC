namespace NutriTec.Infrastructure.Sql.Persistence.Entities;

/*
 * Descripción:
 * Representa la tabla USUARIO dentro del adaptador SQL Server.
 *
 * Entradas:
 * Recibe datos personales, métricas corporales, correo y hash de contraseña desde la persistencia relacional.
 *
 * Salidas:
 * Permite a Entity Framework Core materializar usuarios finales sin exponer la entidad al frontend.
 *
 * Restricciones:
 * Es una entidad de infraestructura; la contraseña debe persistirse únicamente como hash y nunca en texto plano.
 */
public sealed class UsuarioSql
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public int Edad { get; set; }
    public DateOnly FechaNacimiento { get; set; }
    public decimal Peso { get; set; }
    public decimal Imc { get; set; }
    public string Pais { get; set; } = string.Empty;
    public decimal? Cintura { get; set; }
    public decimal? Cuello { get; set; }
    public decimal? Caderas { get; set; }
    public decimal? PctMusculo { get; set; }
    public decimal? PctGrasa { get; set; }
    public decimal CaloriasDiariasMax { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}

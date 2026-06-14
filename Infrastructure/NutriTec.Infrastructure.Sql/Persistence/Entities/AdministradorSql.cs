namespace NutriTec.Infrastructure.Sql.Persistence.Entities;

/*
 * Descripción:
 * Representa la tabla ADMINISTRADOR dentro del adaptador SQL Server.
 *
 * Entradas:
 * Recibe email y hash de contraseña desde la persistencia relacional.
 *
 * Salidas:
 * Permite a Entity Framework Core materializar credenciales de administradores.
 *
 * Restricciones:
 * Es una entidad de infraestructura; no debe exponerse directamente desde controllers ni contratos públicos.
 */
public sealed class AdministradorSql
{
    public int IdAdmin { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}

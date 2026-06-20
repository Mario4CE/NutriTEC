namespace NutriTec.Infrastructure.Sql.Persistence.Entities;

public sealed class AdministradorSql
{
    public int IdAdmin { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}

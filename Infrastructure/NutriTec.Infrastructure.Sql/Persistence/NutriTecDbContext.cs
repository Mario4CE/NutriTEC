using Microsoft.EntityFrameworkCore;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Persistence;

public sealed class NutriTecDbContext(DbContextOptions<NutriTecDbContext> options) : DbContext(options)
{
    public DbSet<AdministradorSql> Administradores => Set<AdministradorSql>();
    public DbSet<UsuarioSql> Usuarios => Set<UsuarioSql>();
    public DbSet<NutricionistaSql> Nutricionistas => Set<NutricionistaSql>();
}

using Microsoft.EntityFrameworkCore;
using NutriTec.Domain.Productos;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Persistence;

public sealed class NutriTecDbContext(DbContextOptions<NutriTecDbContext> options) : DbContext(options)
{
    public DbSet<AdministradorSql> Administradores => Set<AdministradorSql>();
    public DbSet<TipoCobroSql> TiposCobro => Set<TipoCobroSql>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<UsuarioSql> Usuarios => Set<UsuarioSql>();
    public DbSet<NutricionistaSql> Nutricionistas => Set<NutricionistaSql>();

    /*
    Descripción:
    Aplica las configuraciones relacionales definidas para las entidades SQL de Infrastructure.

    Entradas:
    ModelBuilder proporcionado por Entity Framework Core al construir el modelo.

    Salidas:
    Modelo EF Core configurado con tablas, columnas, longitudes, precisiones e índices definidos en este assembly.

    Restricciones:
    No crea ni modifica scripts SQL; solo asegura que EF Core use las configuraciones ya versionadas en Infrastructure.Sql.
    */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NutriTecDbContext).Assembly);
    }
}

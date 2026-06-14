using Microsoft.EntityFrameworkCore;
using NutriTec.Domain.Productos;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Persistence;

/*
 * Descripción:
 * Representa la sesión de Entity Framework Core utilizada por la infraestructura SQL Server de NutriTEC.
 *
 * Entradas:
 * Recibe las opciones de configuración preparadas por el contenedor de inyección de dependencias.
 *
 * Salidas:
 * Proporciona acceso relacional a los agregados SQL configurados.
 *
 * Restricciones:
 * Debe utilizarse únicamente desde Infrastructure.Sql; los controllers no deben acceder directamente al contexto.
 */
public sealed class NutriTecDbContext(DbContextOptions<NutriTecDbContext> options) : DbContext(options)
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<AdministradorSql> Administradores => Set<AdministradorSql>();
    public DbSet<UsuarioSql> Usuarios => Set<UsuarioSql>();
    public DbSet<NutricionistaSql> Nutricionistas => Set<NutricionistaSql>();

    /*
     * Descripción:
     * Aplica automáticamente las configuraciones relacionales del ensamblado.
     * Entradas:
     * Recibe el constructor del modelo EF Core.
     * Salidas:
     * Completa el modelo utilizado por NutriTecDbContext.
     * Restricciones:
     * Las configuraciones deben implementar IEntityTypeConfiguration<T>.
     */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NutriTecDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

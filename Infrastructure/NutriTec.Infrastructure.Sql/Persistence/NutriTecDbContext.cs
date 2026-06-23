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
    public DbSet<PacienteNutricionistaSql> PacientesNutricionista => Set<PacienteNutricionistaSql>();
    public DbSet<PlanAlimentacionSql> PlanesAlimentacion => Set<PlanAlimentacionSql>();
    public DbSet<TiempoComidaPlanSql> TiemposComidaPlan => Set<TiempoComidaPlanSql>();
    public DbSet<PlanProductoSql> PlanesProducto => Set<PlanProductoSql>();
    public DbSet<AsignacionPlanSql> AsignacionesPlan => Set<AsignacionPlanSql>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NutriTecDbContext).Assembly);
    }
}
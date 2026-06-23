using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Persistence.Configurations;

/*
 * Descripción:
 * Configura el mapeo relacional de PlanAlimentacionSql para SQL Server.
 *
 * Entradas:
 * Recibe el constructor de entidad proporcionado por Entity Framework Core.
 *
 * Salidas:
 * Define tabla, llave primaria autoincremental y llave foránea hacia NUTRICIONISTA.
 *
 * Restricciones:
 * total_calorias se recalcula automáticamente mediante el trigger trg_RecalcularTotalPlan;
 * no debe escribirse manualmente desde la capa de aplicación.
 */
public sealed class PlanAlimentacionSqlConfiguration : IEntityTypeConfiguration<PlanAlimentacionSql>
{
    public void Configure(EntityTypeBuilder<PlanAlimentacionSql> builder)
    {
        builder.ToTable("PLAN_ALIMENTACION");
        builder.HasKey(plan => plan.IdPlan);

        builder.Property(plan => plan.IdPlan)
            .HasColumnName("id_plan")
            .ValueGeneratedOnAdd();

        builder.Property(plan => plan.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(150)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(plan => plan.CedulaNutricionista)
            .HasColumnName("cedula_nutricionista")
            .HasMaxLength(20)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(plan => plan.TotalCalorias)
            .HasColumnName("total_calorias")
            .HasPrecision(7, 2);

        builder.HasOne<NutricionistaSql>()
            .WithMany()
            .HasForeignKey(plan => plan.CedulaNutricionista)
            .HasPrincipalKey(nutricionista => nutricionista.Cedula)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(plan => plan.Tiempos)
            .WithOne()
            .HasForeignKey(tiempo => tiempo.IdPlan)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/*
 * Descripción:
 * Configura el mapeo relacional de TiempoComidaPlanSql para SQL Server.
 *
 * Entradas:
 * Recibe el constructor de entidad proporcionado por Entity Framework Core.
 *
 * Salidas:
 * Define tabla, llave primaria autoincremental, restricción de unicidad por plan y tipo
 * de comida, y llave foránea hacia PLAN_ALIMENTACION.
 *
 * Restricciones:
 * tipo_comida debe coincidir exactamente con los valores aceptados por el CHECK de SQL Server.
 */
public sealed class TiempoComidaPlanSqlConfiguration : IEntityTypeConfiguration<TiempoComidaPlanSql>
{
    public void Configure(EntityTypeBuilder<TiempoComidaPlanSql> builder)
    {
        builder.ToTable("TIEMPO_COMIDA_PLAN");
        builder.HasKey(tiempo => tiempo.IdTiempo);

        builder.Property(tiempo => tiempo.IdTiempo)
            .HasColumnName("id_tiempo")
            .ValueGeneratedOnAdd();

        builder.Property(tiempo => tiempo.IdPlan)
            .HasColumnName("id_plan")
            .IsRequired();

        builder.Property(tiempo => tiempo.TipoComida)
            .HasColumnName("tipo_comida")
            .HasMaxLength(20)
            .IsUnicode(false)
            .IsRequired();

        builder.HasIndex(tiempo => new { tiempo.IdPlan, tiempo.TipoComida }).IsUnique();

        builder.HasMany(tiempo => tiempo.Productos)
            .WithOne()
            .HasForeignKey(producto => producto.IdTiempo)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/*
 * Descripción:
 * Configura el mapeo relacional de PlanProductoSql para SQL Server.
 *
 * Entradas:
 * Recibe el constructor de entidad proporcionado por Entity Framework Core.
 *
 * Salidas:
 * Define tabla, llave primaria compuesta y llaves foráneas hacia TIEMPO_COMIDA_PLAN y PRODUCTO.
 *
 * Restricciones:
 * cantidad_porciones debe ser mayor que cero; la validación se aplica desde Application.
 */
public sealed class PlanProductoSqlConfiguration : IEntityTypeConfiguration<PlanProductoSql>
{
    public void Configure(EntityTypeBuilder<PlanProductoSql> builder)
    {
        builder.ToTable("PLAN_PRODUCTO");
        builder.HasKey(producto => new { producto.IdTiempo, producto.IdProducto });

        builder.Property(producto => producto.IdTiempo)
            .HasColumnName("id_tiempo")
            .IsRequired();

        builder.Property(producto => producto.IdProducto)
            .HasColumnName("id_producto")
            .IsRequired();

        builder.Property(producto => producto.CantidadPorciones)
            .HasColumnName("cantidad_porciones")
            .HasPrecision(5, 2)
            .IsRequired();
    }
}
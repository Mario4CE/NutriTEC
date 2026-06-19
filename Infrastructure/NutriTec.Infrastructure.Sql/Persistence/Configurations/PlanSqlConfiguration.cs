using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Persistence.Configurations;

public sealed class PlanSqlConfiguration : IEntityTypeConfiguration<PlanSql>
{
    public void Configure(EntityTypeBuilder<PlanSql> builder)
    {
        builder.ToTable("PLAN");
        builder.HasKey(plan => plan.Id);

        builder.Property(plan => plan.Id)
            .HasColumnName("id_plan")
            .ValueGeneratedNever();

        builder.Property(plan => plan.IdNutricionista)
            .HasColumnName("id_nutricionista")
            .IsRequired();

        builder.Property(plan => plan.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(150)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(plan => plan.FechaCreacionUtc)
            .HasColumnName("fecha_creacion_utc")
            .IsRequired();
    }
}

public sealed class ItemPlanSqlConfiguration : IEntityTypeConfiguration<ItemPlanSql>
{
    public void Configure(EntityTypeBuilder<ItemPlanSql> builder)
    {
        builder.ToTable("ITEM_PLAN");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id)
            .HasColumnName("id_item_plan")
            .ValueGeneratedNever();

        builder.Property(item => item.IdPlan)
            .HasColumnName("id_plan")
            .IsRequired();

        builder.Property(item => item.TiempoComida)
            .HasColumnName("tiempo_comida")
            .IsRequired();

        builder.Property(item => item.IdProducto)
            .HasColumnName("id_producto")
            .IsRequired();

        builder.Property(item => item.Porciones)
            .HasColumnName("porciones")
            .HasPrecision(8, 2)
            .IsRequired();

        builder.HasOne<PlanSql>()
            .WithMany(plan => plan.Items)
            .HasForeignKey(item => item.IdPlan)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
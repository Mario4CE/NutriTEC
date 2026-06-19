using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Persistence.Configurations;

public sealed class AsignacionPlanSqlConfiguration : IEntityTypeConfiguration<AsignacionPlanSql>
{
    public void Configure(EntityTypeBuilder<AsignacionPlanSql> builder)
    {
        builder.ToTable("ASIGNACION_PLAN");
        builder.HasKey(asignacion => asignacion.Id);

        builder.Property(asignacion => asignacion.Id)
            .HasColumnName("id_asignacion_plan")
            .ValueGeneratedNever();

        builder.Property(asignacion => asignacion.IdPaciente)
            .HasColumnName("id_paciente")
            .IsRequired();

        builder.Property(asignacion => asignacion.IdPlan)
            .HasColumnName("id_plan")
            .IsRequired();

        builder.Property(asignacion => asignacion.IdNutricionista)
            .HasColumnName("id_nutricionista")
            .IsRequired();

        builder.Property(asignacion => asignacion.FechaInicio)
            .HasColumnName("fecha_inicio")
            .IsRequired();

        builder.Property(asignacion => asignacion.FechaFin)
            .HasColumnName("fecha_fin")
            .IsRequired();

        builder.Property(asignacion => asignacion.FechaAsignacionUtc)
            .HasColumnName("fecha_asignacion_utc")
            .IsRequired();
    }
}
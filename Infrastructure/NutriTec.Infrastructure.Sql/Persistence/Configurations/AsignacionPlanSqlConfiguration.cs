using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Persistence.Configurations;

/*
 * Descripción:
 * Configura el mapeo relacional de AsignacionPlanSql para SQL Server.
 *
 * Entradas:
 * Recibe el constructor de entidad proporcionado por Entity Framework Core.
 *
 * Salidas:
 * Define tabla, llave primaria autoincremental y llaves foráneas hacia PLAN_ALIMENTACION y USUARIO.
 *
 * Restricciones:
 * Debe mantenerse alineado con Database/SqlServer/Complete/TablaCompleta.sql; el rango
 * de fechas se valida desde Application antes de persistir.
 */
public sealed class AsignacionPlanSqlConfiguration : IEntityTypeConfiguration<AsignacionPlanSql>
{
    public void Configure(EntityTypeBuilder<AsignacionPlanSql> builder)
    {
        builder.ToTable("ASIGNACION_PLAN");
        builder.HasKey(asignacion => asignacion.IdAsignacion);

        builder.Property(asignacion => asignacion.IdAsignacion)
            .HasColumnName("id_asignacion")
            .ValueGeneratedOnAdd();

        builder.Property(asignacion => asignacion.IdPlan)
            .HasColumnName("id_plan")
            .IsRequired();

        builder.Property(asignacion => asignacion.IdUsuario)
            .HasColumnName("id_usuario")
            .IsRequired();

        builder.Property(asignacion => asignacion.FechaInicio)
            .HasColumnName("fecha_inicio")
            .IsRequired();

        builder.Property(asignacion => asignacion.FechaFin)
            .HasColumnName("fecha_fin")
            .IsRequired();

        builder.HasOne<PlanAlimentacionSql>()
            .WithMany()
            .HasForeignKey(asignacion => asignacion.IdPlan)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UsuarioSql>()
            .WithMany()
            .HasForeignKey(asignacion => asignacion.IdUsuario)
            .HasPrincipalKey(usuario => usuario.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Persistence.Configurations;

/*
 * Descripción:
 * Configura el mapeo relacional de PacienteNutricionistaSql para SQL Server.
 *
 * Entradas:
 * Recibe el constructor de entidad proporcionado por Entity Framework Core.
 *
 * Salidas:
 * Define tabla, llave primaria compuesta y llaves foráneas hacia NUTRICIONISTA y USUARIO.
 *
 * Restricciones:
 * Debe mantenerse alineado con Database/SqlServer/Complete/TablaCompleta.sql.
 */
public sealed class PacienteNutricionistaConfiguration : IEntityTypeConfiguration<PacienteNutricionistaSql>
{
    public void Configure(EntityTypeBuilder<PacienteNutricionistaSql> builder)
    {
        builder.ToTable("PACIENTE_NUTRICIONISTA");
        builder.HasKey(paciente => new { paciente.CedulaNutricionista, paciente.IdUsuario });

        builder.Property(paciente => paciente.CedulaNutricionista)
            .HasColumnName("cedula_nutricionista")
            .HasMaxLength(20)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(paciente => paciente.IdUsuario)
            .HasColumnName("id_usuario")
            .IsRequired();

        builder.HasOne<NutricionistaSql>()
            .WithMany()
            .HasForeignKey(paciente => paciente.CedulaNutricionista)
            .HasPrincipalKey(nutricionista => nutricionista.Cedula)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UsuarioSql>()
            .WithMany()
            .HasForeignKey(paciente => paciente.IdUsuario)
            .HasPrincipalKey(usuario => usuario.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
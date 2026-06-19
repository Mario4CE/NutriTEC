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
 * Define tabla, clave primaria, columnas e índice único de asociación.
 *
 * Restricciones:
 * Debe mantenerse alineado con Database/SqlServer/Tables/PACIENTE_NUTRICIONISTA.sql.
 * No declara FK formal a NUTRICIONISTA.IdNutricionista porque ese campo no es su llave
 * primaria (la llave primaria de NUTRICIONISTA es Cedula); la integridad se valida desde
 * la capa de aplicación mediante IUsuarioConsultaRepository y IPacienteRepository.
 */
public sealed class PacienteNutricionistaConfiguration : IEntityTypeConfiguration<PacienteNutricionistaSql>
{
    public void Configure(EntityTypeBuilder<PacienteNutricionistaSql> builder)
    {
        builder.ToTable("PACIENTE_NUTRICIONISTA");
        builder.HasKey(paciente => paciente.Id);

        builder.Property(paciente => paciente.Id)
            .HasColumnName("id_paciente_nutricionista")
            .ValueGeneratedNever();

        builder.Property(paciente => paciente.IdNutricionista)
            .HasColumnName("id_nutricionista")
            .IsRequired();

        builder.Property(paciente => paciente.IdPaciente)
            .HasColumnName("id_paciente")
            .IsRequired();

        builder.Property(paciente => paciente.FechaAsociacionUtc)
            .HasColumnName("fecha_asociacion_utc")
            .IsRequired();

        builder.HasIndex(paciente => new { paciente.IdNutricionista, paciente.IdPaciente })
            .IsUnique();
    }
}
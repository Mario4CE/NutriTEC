using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Persistence.Configurations;

/*
 * Descripción:
 * Configura el mapeo relacional de NutricionistaSql para SQL Server.
 *
 * Entradas:
 * Recibe el constructor de entidad proporcionado por Entity Framework Core.
 *
 * Salidas:
 * Define tabla, clave primaria, columnas, longitudes, precisiones, índices únicos y FK al catálogo TIPO_COBRO.
 *
 * Restricciones:
 * Debe mantenerse alineado con Database/SqlServer/Tables/002_NUTRICIONISTA.sql.
 */
public sealed class NutricionistaSqlConfiguration : IEntityTypeConfiguration<NutricionistaSql>
{
    /*
     * Descripción:
     * Aplica el mapeo de NUTRICIONISTA al modelo EF Core.
     * Entradas:
     * Recibe el constructor relacional de la entidad.
     * Salidas:
     * Configura tabla, propiedades, índices únicos y relación con TIPO_COBRO.
     * Restricciones:
     * No debe mapear ni exponer contraseñas en texto plano.
     */
    public void Configure(EntityTypeBuilder<NutricionistaSql> builder)
    {
        builder.ToTable("NUTRICIONISTA");
        builder.HasKey(nutricionista => nutricionista.Cedula);

        builder.Property(nutricionista => nutricionista.Cedula).HasColumnName("cedula").HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(nutricionista => nutricionista.Nombre).HasColumnName("nombre").HasMaxLength(100).IsUnicode(false).IsRequired();
        builder.Property(nutricionista => nutricionista.Apellidos).HasColumnName("apellidos").HasMaxLength(100).IsUnicode(false).IsRequired();
        builder.Property(nutricionista => nutricionista.CodigoNutricionista).HasColumnName("codigo_nutricionista").HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.Property(nutricionista => nutricionista.Edad).HasColumnName("edad").IsRequired();
        builder.Property(nutricionista => nutricionista.FechaNacimiento).HasColumnName("fecha_nacimiento").IsRequired();
        builder.Property(nutricionista => nutricionista.Peso).HasColumnName("peso").HasPrecision(5, 2).IsRequired();
        builder.Property(nutricionista => nutricionista.Imc).HasColumnName("imc").HasPrecision(5, 2).IsRequired();
        builder.Property(nutricionista => nutricionista.Direccion).HasColumnName("direccion").HasMaxLength(255).IsUnicode(false).IsRequired();
        builder.Property(nutricionista => nutricionista.FotoUrl).HasColumnName("foto_url").HasMaxLength(500).IsUnicode(false);
        builder.Property(nutricionista => nutricionista.TarjetaCredito).HasColumnName("tarjeta_credito").HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(nutricionista => nutricionista.TipoCobro).HasColumnName("tipo_cobro").HasMaxLength(10).IsUnicode(false).IsRequired();
        builder.Property(nutricionista => nutricionista.Email).HasColumnName("email").HasMaxLength(100).IsUnicode(false).IsRequired();
        builder.Property(nutricionista => nutricionista.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsUnicode(false).IsRequired();

        builder.HasIndex(nutricionista => nutricionista.CodigoNutricionista)
            .IsUnique();
        builder.HasIndex(nutricionista => nutricionista.Email)
            .IsUnique();

        builder.HasOne<TipoCobroSql>()
            .WithMany()
            .HasForeignKey(nutricionista => nutricionista.TipoCobro)
            .HasPrincipalKey(tipoCobro => tipoCobro.CodigoTipoCobro)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

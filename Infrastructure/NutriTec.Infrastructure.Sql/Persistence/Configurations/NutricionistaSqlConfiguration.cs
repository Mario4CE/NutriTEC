using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Persistence.Configurations;

public sealed class NutricionistaSqlConfiguration : IEntityTypeConfiguration<NutricionistaSql>
{
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

        builder.HasIndex(nutricionista => nutricionista.CodigoNutricionista).IsUnique();
        builder.HasIndex(nutricionista => nutricionista.Email).IsUnique();

        builder.HasOne<TipoCobroSql>()
            .WithMany()
            .HasForeignKey(nutricionista => nutricionista.TipoCobro)
            .HasPrincipalKey(tipoCobro => tipoCobro.CodigoTipoCobro)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
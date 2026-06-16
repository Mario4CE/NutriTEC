using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Persistence.Configurations;

/*
 * Descripción:
 * Configura el mapeo relacional del catálogo TipoCobroSql para SQL Server.
 *
 * Entradas:
 * Recibe el constructor de entidad proporcionado por Entity Framework Core.
 *
 * Salidas:
 * Define tabla, clave primaria, columnas y restricciones del catálogo TIPO_COBRO.
 *
 * Restricciones:
 * Debe mantenerse alineado con Database/SqlServer/Tables/000_TIPO_COBRO.sql.
 */
public sealed class TipoCobroSqlConfiguration : IEntityTypeConfiguration<TipoCobroSql>
{
    public void Configure(EntityTypeBuilder<TipoCobroSql> builder)
    {
        builder.ToTable("TIPO_COBRO");
        builder.HasKey(tipoCobro => tipoCobro.CodigoTipoCobro);

        builder.Property(tipoCobro => tipoCobro.CodigoTipoCobro)
            .HasColumnName("codigo_tipo_cobro")
            .HasMaxLength(10)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(tipoCobro => tipoCobro.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(50)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(tipoCobro => tipoCobro.Activo)
            .HasColumnName("activo")
            .HasDefaultValue(true)
            .IsRequired();
    }
}

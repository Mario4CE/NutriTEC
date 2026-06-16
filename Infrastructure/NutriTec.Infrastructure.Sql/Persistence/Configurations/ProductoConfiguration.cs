using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTec.Domain.Productos;

namespace NutriTec.Infrastructure.Sql.Persistence.Configurations;

/*
 * Descripción:
 * Configura el mapeo relacional de Producto para SQL Server.
 *
 * Entradas:
 * Recibe el constructor de entidad proporcionado por Entity Framework Core.
 *
 * Salidas:
 * Define tabla, clave primaria por identificador, columnas, precisiones e índice único de código de barras.
 *
 * Restricciones:
 * Debe mantenerse alineado con el script Database/SqlServer/Tables/006_PRODUCTO.sql.
 */
public sealed class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    /*
     * Descripción:
     * Aplica el mapeo de Producto al modelo EF Core.
     * Entradas:
     * Recibe el constructor relacional de la entidad.
     * Salidas:
     * Configura tabla, propiedades e índice único.
     * Restricciones:
     * Debe mantenerse alineado con el script SQL versionado.
     */
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("PRODUCTO");
        builder.HasKey(producto => producto.Id);

        builder.Property(producto => producto.Id)
            .HasColumnName("id_producto")
            .ValueGeneratedNever();

        builder.Property(producto => producto.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(150)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(producto => producto.CodigoBarras)
            .HasColumnName("codigo_barras")
            .HasMaxLength(64)
            .IsUnicode(false)
            .IsRequired();

        builder.HasIndex(producto => producto.CodigoBarras).IsUnique();

        builder.Property(producto => producto.Calorias)
            .HasColumnName("calorias")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(producto => producto.Proteinas)
            .HasColumnName("proteinas")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(producto => producto.Carbohidratos)
            .HasColumnName("carbohidratos")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(producto => producto.Grasas)
            .HasColumnName("grasas")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(producto => producto.EstaAprobado)
            .HasColumnName("aprobado")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(producto => producto.FechaCreacionUtc)
            .HasColumnName("fecha_creacion_utc")
            .IsRequired();
    }
}

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
 * Debe mantenerse alineado con Database/SqlServer/Complete/TablaCompleta.sql. La tabla real
 * no incluye porcion_g_ml, sodio_mg, vitaminas, calcio_mg ni hierro_mg; esas propiedades del
 * dominio se marcan como no persistidas (Ignore) hasta que el equipo decida si se agregan
 * a la tabla PRODUCTO.
 */
public sealed class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
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

        builder.Property(producto => producto.PorcionGramosMililitros)
            .HasColumnName("porcion_g_ml")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(producto => producto.SodioMiligramos)
            .HasColumnName("sodio_mg")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(producto => producto.Vitaminas)
            .HasColumnName("vitaminas")
            .HasMaxLength(255)
            .IsUnicode(false);

        builder.Property(producto => producto.CalcioMiligramos)
            .HasColumnName("calcio_mg")
            .HasPrecision(10, 2);

        builder.Property(producto => producto.HierroMiligramos)
            .HasColumnName("hierro_mg")
            .HasPrecision(10, 2);
            }
}
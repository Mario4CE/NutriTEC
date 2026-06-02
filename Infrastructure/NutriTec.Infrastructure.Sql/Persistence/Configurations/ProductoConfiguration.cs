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
 * Define tabla, clave primaria, longitudes, precisiones e índice único de código de barras.
 *
 * Restricciones:
 * Debe mantenerse alineado con el script Database/SqlServer/Tables/001_Productos.sql.
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
        builder.ToTable("Productos");
        builder.HasKey(producto => producto.Id);
        builder.Property(producto => producto.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(producto => producto.CodigoBarras).HasMaxLength(64).IsRequired();
        builder.HasIndex(producto => producto.CodigoBarras).IsUnique();
        builder.Property(producto => producto.Calorias).HasPrecision(10, 2);
        builder.Property(producto => producto.Proteinas).HasPrecision(10, 2);
        builder.Property(producto => producto.Carbohidratos).HasPrecision(10, 2);
        builder.Property(producto => producto.Grasas).HasPrecision(10, 2);
        builder.Property(producto => producto.EstaAprobado).HasDefaultValue(false);
    }
}

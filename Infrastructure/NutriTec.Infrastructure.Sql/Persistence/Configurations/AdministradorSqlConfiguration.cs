using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Persistence.Configurations;

/*
 * Descripción:
 * Configura el mapeo relacional de AdministradorSql para SQL Server.
 *
 * Entradas:
 * Recibe el constructor de entidad proporcionado por Entity Framework Core.
 *
 * Salidas:
 * Define tabla, clave primaria, columnas, longitudes e índice único de email.
 *
 * Restricciones:
 * Debe mantenerse alineado con Database/SqlServer/Tables/001_ADMINISTRADOR.sql.
 */
public sealed class AdministradorSqlConfiguration : IEntityTypeConfiguration<AdministradorSql>
{
    /*
     * Descripción:
     * Aplica el mapeo de ADMINISTRADOR al modelo EF Core.
     * Entradas:
     * Recibe el constructor relacional de la entidad.
     * Salidas:
     * Configura tabla, propiedades e índice único.
     * Restricciones:
     * No debe mapear ni exponer contraseñas en texto plano.
     */
    public void Configure(EntityTypeBuilder<AdministradorSql> builder)
    {
        builder.ToTable("ADMINISTRADOR");
        builder.HasKey(administrador => administrador.IdAdmin);

        builder.Property(administrador => administrador.IdAdmin)
            .HasColumnName("id_admin");

        builder.Property(administrador => administrador.Email)
            .HasColumnName("email")
            .HasMaxLength(100)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(administrador => administrador.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(255)
            .IsUnicode(false)
            .IsRequired();

        builder.HasIndex(administrador => administrador.Email)
            .IsUnique();
    }
}

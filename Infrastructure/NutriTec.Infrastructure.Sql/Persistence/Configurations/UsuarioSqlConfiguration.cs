using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Persistence.Configurations;

/*
 * Descripción:
 * Configura el mapeo relacional de UsuarioSql para SQL Server.
 *
 * Entradas:
 * Recibe el constructor de entidad proporcionado por Entity Framework Core.
 *
 * Salidas:
 * Define tabla, clave primaria, columnas, longitudes, precisiones e índice único de email.
 *
 * Restricciones:
 * Debe mantenerse alineado con Database/SqlServer/Tables/003_USUARIO.sql.
 */
public sealed class UsuarioSqlConfiguration : IEntityTypeConfiguration<UsuarioSql>
{
    /*
     * Descripción:
     * Aplica el mapeo de USUARIO al modelo EF Core.
     * Entradas:
     * Recibe el constructor relacional de la entidad.
     * Salidas:
     * Configura tabla, propiedades e índice único.
     * Restricciones:
     * No debe mapear ni exponer contraseñas en texto plano.
     */
    public void Configure(EntityTypeBuilder<UsuarioSql> builder)
    {
        builder.ToTable("USUARIO");
        builder.HasKey(usuario => usuario.IdUsuario);

        builder.Property(usuario => usuario.IdUsuario).HasColumnName("id_usuario");
        builder.Property(usuario => usuario.Nombre).HasColumnName("nombre").HasMaxLength(100).IsUnicode(false).IsRequired();
        builder.Property(usuario => usuario.Apellidos).HasColumnName("apellidos").HasMaxLength(100).IsUnicode(false).IsRequired();
        builder.Property(usuario => usuario.Edad).HasColumnName("edad").IsRequired();
        builder.Property(usuario => usuario.FechaNacimiento).HasColumnName("fecha_nacimiento").IsRequired();
        builder.Property(usuario => usuario.Peso).HasColumnName("peso").HasPrecision(5, 2).IsRequired();
        builder.Property(usuario => usuario.Imc).HasColumnName("imc").HasPrecision(5, 2).IsRequired();
        builder.Property(usuario => usuario.Pais).HasColumnName("pais").HasMaxLength(100).IsUnicode(false).IsRequired();
        builder.Property(usuario => usuario.Cintura).HasColumnName("cintura").HasPrecision(5, 2);
        builder.Property(usuario => usuario.Cuello).HasColumnName("cuello").HasPrecision(5, 2);
        builder.Property(usuario => usuario.Caderas).HasColumnName("caderas").HasPrecision(5, 2);
        builder.Property(usuario => usuario.PctMusculo).HasColumnName("pct_musculo").HasPrecision(5, 2);
        builder.Property(usuario => usuario.PctGrasa).HasColumnName("pct_grasa").HasPrecision(5, 2);
        builder.Property(usuario => usuario.CaloriasDiariasMax).HasColumnName("calorias_diarias_max").HasPrecision(7, 2).IsRequired();
        builder.Property(usuario => usuario.Email).HasColumnName("email").HasMaxLength(100).IsUnicode(false).IsRequired();
        builder.Property(usuario => usuario.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsUnicode(false).IsRequired();

        builder.HasIndex(usuario => usuario.Email)
            .IsUnique();
    }
}

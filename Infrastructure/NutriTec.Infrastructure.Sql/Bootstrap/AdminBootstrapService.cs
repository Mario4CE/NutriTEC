using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Infrastructure.Sql.Persistence;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Bootstrap;

/*
Descripción:
Crea de forma idempotente el primer administrador del sistema cuando el bootstrap está habilitado por configuración.

Entradas:
NutriTecDbContext para persistencia SQL, IPasswordHasher para generar el hash seguro y AdminBootstrapOptions para correo/contraseña temporal.

Salidas:
Inserta un administrador inicial solo si no existe ningún administrador previamente registrado.

Restricciones:
No expone contraseñas ni hashes, no registra datos sensibles en logs, no crea datos de negocio y no modifica scripts bajo Database/.
*/
public sealed class AdminBootstrapService(
    NutriTecDbContext dbContext,
    IPasswordHasher passwordHasher,
    IOptions<AdminBootstrapOptions> options)
{
    public async Task InicializarAsync(CancellationToken cancellationToken = default)
    {
        var bootstrapOptions = options.Value;

        if (!bootstrapOptions.Enabled)
        {
            return;
        }

        var email = bootstrapOptions.Email.Trim();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(bootstrapOptions.Password))
        {
            throw new InvalidOperationException("La configuración de bootstrap del administrador está incompleta.");
        }

        if (await dbContext.Administradores.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.Administradores.Add(new AdministradorSql
        {
            Email = email,
            PasswordHash = passwordHasher.GenerarHash(bootstrapOptions.Password)
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

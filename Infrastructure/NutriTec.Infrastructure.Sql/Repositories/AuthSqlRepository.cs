using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Common;
using NutriTec.Infrastructure.Sql.Persistence;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Repositories;

public sealed class AuthSqlRepository(NutriTecDbContext dbContext) : IAuthRepository
{
    private const string TipoCliente = "Cliente";
    private const string TipoNutricionista = "Nutricionista";
    private const string TipoAdministrador = "Administrador";

    public async Task<CredencialAutenticacion?> ObtenerCredencialPorCorreoAsync(string correo, CancellationToken cancellationToken)
    {
        var correoNormalizado = NormalizarCorreo(correo);

        var usuario = await dbContext.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(usuario => usuario.Email == correoNormalizado, cancellationToken);

        if (usuario is not null)
        {
            return MapearUsuario(usuario);
        }

        var nutricionista = await dbContext.Nutricionistas
            .AsNoTracking()
            .FirstOrDefaultAsync(nutricionista => nutricionista.Email == correoNormalizado, cancellationToken);

        if (nutricionista is not null)
        {
            return MapearNutricionista(nutricionista);
        }

        var administrador = await dbContext.Administradores
            .AsNoTracking()
            .FirstOrDefaultAsync(administrador => administrador.Email == correoNormalizado, cancellationToken);

        return administrador is null ? null : MapearAdministrador(administrador);
    }

    public async Task<bool> ExisteCorreoAsync(string correo, CancellationToken cancellationToken)
    {
        var correoNormalizado = NormalizarCorreo(correo);

        return await dbContext.Usuarios.AnyAsync(usuario => usuario.Email == correoNormalizado, cancellationToken)
            || await dbContext.Nutricionistas.AnyAsync(nutricionista => nutricionista.Email == correoNormalizado, cancellationToken)
            || await dbContext.Administradores.AnyAsync(administrador => administrador.Email == correoNormalizado, cancellationToken);
    }

    public async Task<CredencialAutenticacion> RegistrarUsuarioAsync(NuevoUsuarioAutenticacion usuario, CancellationToken cancellationToken)
    {
        return usuario.TipoUsuario switch
        {
            TipoCliente => await RegistrarClienteAsync(usuario, cancellationToken),
            TipoNutricionista => await RegistrarNutricionistaAsync(usuario, cancellationToken),
            _ => throw new InvalidOperationException($"El tipo de usuario '{usuario.TipoUsuario}' no está soportado para registro SQL.")
        };
    }

    private async Task<CredencialAutenticacion> RegistrarClienteAsync(NuevoUsuarioAutenticacion usuario, CancellationToken cancellationToken)
    {
        var entidad = new UsuarioSql
        {
            Nombre = usuario.Nombre,
            Apellidos = usuario.Apellidos,
            Edad = usuario.Edad,
            FechaNacimiento = usuario.FechaNacimiento,
            Peso = usuario.Peso,
            Imc = usuario.Imc,
            Pais = usuario.Pais ?? string.Empty,
            Cintura = usuario.Cintura,
            Cuello = usuario.Cuello,
            Caderas = usuario.Caderas,
            PctMusculo = usuario.PctMusculo,
            PctGrasa = usuario.PctGrasa,
            CaloriasDiariasMax = usuario.CaloriasDiariasMax ?? 0,
            Email = NormalizarCorreo(usuario.Correo),
            PasswordHash = usuario.ContrasenaHash
        };

        dbContext.Usuarios.Add(entidad);
        await GuardarCambiosTraduciendoConflictosAsync(cancellationToken);

        return MapearUsuario(entidad);
    }

    private async Task<CredencialAutenticacion> RegistrarNutricionistaAsync(NuevoUsuarioAutenticacion usuario, CancellationToken cancellationToken)
    {
        var entidad = new NutricionistaSql
        {
            Cedula = usuario.Cedula ?? throw new InvalidOperationException("La cédula es obligatoria para registrar nutricionistas."),
            Nombre = usuario.Nombre,
            Apellidos = usuario.Apellidos,
            CodigoNutricionista = usuario.CodigoNutricionista ?? throw new InvalidOperationException("El código de nutricionista es obligatorio."),
            Edad = usuario.Edad,
            FechaNacimiento = usuario.FechaNacimiento,
            Peso = usuario.Peso,
            Imc = usuario.Imc,
            Direccion = usuario.Direccion ?? string.Empty,
            FotoUrl = usuario.FotoUrl,
            TarjetaCredito = usuario.TarjetaCredito ?? string.Empty,
            TipoCobro = usuario.TipoCobro ?? string.Empty,
            Email = NormalizarCorreo(usuario.Correo),
            PasswordHash = usuario.ContrasenaHash
        };

        dbContext.Nutricionistas.Add(entidad);
        await GuardarCambiosTraduciendoConflictosAsync(cancellationToken);

        return MapearNutricionista(entidad);
    }

    private async Task GuardarCambiosTraduciendoConflictosAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (EsViolacionUniqueSqlServer(exception))
        {
            throw new ConflictoException("El correo ya está registrado.");
        }
    }

    private static CredencialAutenticacion MapearUsuario(UsuarioSql usuario) => new(
        usuario.IdUsuario.ToString(),
        usuario.Nombre,
        usuario.Email,
        usuario.PasswordHash,
        TipoCliente);

    private static CredencialAutenticacion MapearNutricionista(NutricionistaSql nutricionista) => new(
        nutricionista.Cedula,
        nutricionista.Nombre,
        nutricionista.Email,
        nutricionista.PasswordHash,
        TipoNutricionista);

    private static CredencialAutenticacion MapearAdministrador(AdministradorSql administrador) => new(
        administrador.IdAdmin.ToString(),
        "Administrador",
        administrador.Email,
        administrador.PasswordHash,
        TipoAdministrador);

    private static string NormalizarCorreo(string correo) => correo.Trim();

    private static bool EsViolacionUniqueSqlServer(DbUpdateException exception)
    {
        if (exception.GetBaseException() is SqlException sqlException)
        {
            foreach (SqlError error in sqlException.Errors)
            {
                if (error.Number is 2601 or 2627)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
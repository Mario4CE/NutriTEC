namespace NutriTec.Application.Abstractions.Persistence;

public interface IAuthRepository
{
    Task<CredencialAutenticacion?> ObtenerCredencialPorCorreoAsync(string correo, CancellationToken cancellationToken);

    Task<bool> ExisteCorreoAsync(string correo, CancellationToken cancellationToken);

    Task<CredencialAutenticacion> RegistrarUsuarioAsync(NuevoUsuarioAutenticacion usuario, CancellationToken cancellationToken);
}

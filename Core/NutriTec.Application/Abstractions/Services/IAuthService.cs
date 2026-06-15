using NutriTec.Contracts.Autenticacion;

namespace NutriTec.Application.Abstractions.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

    Task<LoginResponse> RegistrarClienteAsync(RegistrarClienteRequest request, CancellationToken cancellationToken);

    Task<LoginResponse> RegistrarNutricionistaAsync(RegistrarNutricionistaRequest request, CancellationToken cancellationToken);
}

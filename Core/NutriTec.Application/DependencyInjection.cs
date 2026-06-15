using Microsoft.Extensions.DependencyInjection;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Application.Autenticacion;

namespace NutriTec.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddNutriTecApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}

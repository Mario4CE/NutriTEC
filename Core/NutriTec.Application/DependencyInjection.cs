using Microsoft.Extensions.DependencyInjection;

namespace NutriTec.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Aquí puedes registrar los servicios de aplicación, casos de uso (o CQRS/MediatR), validadores, etc.
        // services.AddScoped<IAlgunServicio, AlgunServicio>();

        return services;
    }
}

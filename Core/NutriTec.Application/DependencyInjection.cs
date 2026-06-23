using Microsoft.Extensions.DependencyInjection;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Application.Administracion;
using NutriTec.Application.Autenticacion;
using NutriTec.Application.ObjetosSql;
using NutriTec.Application.Pacientes;
using NutriTec.Application.Planes;
using NutriTec.Application.Productos;
using NutriTec.Application.Retroalimentaciones;

namespace NutriTec.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddNutriTecApplication(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddNutriTecSqlApplication(this IServiceCollection services)
    {
        services.AddNutriTecApplication();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductoService, ProductoService>();
        services.AddScoped<IAdministracionService, AdministracionService>();
        services.AddScoped<IObjetosSqlService, ObjetosSqlService>();
        services.AddScoped<IPacienteService, PacienteService>();
        services.AddScoped<IPlanService, PlanService>();
        services.AddScoped<IAsignacionPlanService, AsignacionPlanService>();

        return services;
    }

    public static IServiceCollection AddNutriTecMongoApplication(this IServiceCollection services)
    {
        services.AddNutriTecApplication();
        services.AddScoped<IRetroalimentacionService, RetroalimentacionService>();

        return services;
    }
}

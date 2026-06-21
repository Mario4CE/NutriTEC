using Microsoft.Extensions.Logging;
using Nutri_TEC.Services;
using Nutri_TEC.Views;

namespace Nutri_TEC
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Registrar servicios
            builder.Services.AddSingleton<IDataService, DataService>();

            // Registrar páginas y view models
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<RegistroPage>();
            builder.Services.AddSingleton<RegistroConsumoPage>();
            builder.Services.AddSingleton<CrearRecetaPage>();
            builder.Services.AddSingleton<MisRecetasPage>();
            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddSingleton<AppShell>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Inicializar datos por defecto
            var dataService = app.Services.GetRequiredService<IDataService>();
            Task.Run(async () =>
            {
                await dataService.InicializarUsuariosDefault();
                await dataService.InicializarProductosDefault();
                await dataService.InicializarRecetasDefault();
            });

            return app;
        }
    }
}

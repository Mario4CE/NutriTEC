using Nutri_TEC.Services;

namespace Nutri_TEC.Views;

public partial class LoginPage : ContentPage
{
    private readonly IDataService _dataService;

    public LoginPage(IDataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            string email = EmailEntry.Text?.Trim();
            string contraseña = ContraseñaEntry.Text?.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contraseña))
            {
                await DisplayAlert("Error", "Por favor ingresa email y contraseña", "OK");
                return;
            }

            var usuario = await _dataService.ValidarLogin(email, contraseña);
            if (usuario == null)
            {
                await DisplayAlert("Error", "Email o contraseña incorrectos", "OK");
                return;
            }

            // Guardar usuario actual en preferencias
            Preferences.Set("UsuarioActual", usuario.Id);
            Preferences.Set("EmailActual", usuario.Email);

            // Navegar al home
            await Shell.Current.GoToAsync("///consumo");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al iniciar sesión: {ex.Message}", "OK");
        }
    }

    private async void OnRegistroClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("registro");
    }
}

using Nutri_TEC.Models;
using Nutri_TEC.Services;

namespace Nutri_TEC.Views;

public partial class RegistroPage : ContentPage
{
    private readonly IDataService _dataService;

    public RegistroPage(IDataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;
    }

    private async void OnRegistrarse(object sender, EventArgs e)
    {
        try
        {
            string email = EmailEntry.Text?.Trim();
            string nombre = NombreEntry.Text?.Trim();
            string contraseña = ContraseñaEntry.Text?.Trim();
            string confirmar = ConfirmarContraseñaEntry.Text?.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(contraseña))
            {
                await DisplayAlert("Error", "Por favor completa todos los campos", "OK");
                return;
            }

            if (contraseña != confirmar)
            {
                await DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
                return;
            }

            if (!int.TryParse(PesoEntry.Text, out int peso) || peso <= 0)
            {
                await DisplayAlert("Error", "Ingresa un peso válido", "OK");
                return;
            }

            if (!int.TryParse(EstaturaEntry.Text, out int estatura) || estatura <= 0)
            {
                await DisplayAlert("Error", "Ingresa una estatura válida", "OK");
                return;
            }

            if (!int.TryParse(EdadEntry.Text, out int edad) || edad <= 0)
            {
                await DisplayAlert("Error", "Ingresa una edad válida", "OK");
                return;
            }

            var usuario = new Usuario
            {
                Email = email,
                Nombre = nombre,
                Contraseña = contraseña,
                Peso = peso,
                Estatura = estatura,
                Edad = edad
            };

            bool registrado = await _dataService.RegistrarUsuario(usuario);
            if (!registrado)
            {
                await DisplayAlert("Error", "El email ya está registrado", "OK");
                return;
            }

            await DisplayAlert("Éxito", "Cuenta creada exitosamente. Por favor inicia sesión", "OK");
            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al registrar usuario: {ex.Message}", "OK");
        }
    }

    private async void OnVolver(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//login");
    }
}

using Nutri_TEC.Models;
using Nutri_TEC.Services;

namespace Nutri_TEC.Views;

public partial class MisRecetasPage : ContentPage
{
    private readonly IDataService _dataService;
    private List<Receta> _recetas;
    private string _usuarioId;

    public MisRecetasPage(IDataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;
        _recetas = new List<Receta>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _usuarioId = Preferences.Get("UsuarioActual", "");
        await CargarRecetas();
    }

    private async Task CargarRecetas()
    {
        _recetas = await _dataService.ObtenerRecetasDelUsuario(_usuarioId);
        
        if (_recetas.Count == 0)
        {
            RecetasView.IsVisible = false;
            EmptyStateView.IsVisible = true;
        }
        else
        {
            RecetasView.IsVisible = true;
            EmptyStateView.IsVisible = false;
            RecetasView.ItemsSource = _recetas;
        }
    }

    private async void OnUsarReceta(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string recetaId)
        {
            var receta = _recetas.FirstOrDefault(r => r.Id == recetaId);
            if (receta != null)
            {
                // Guardar receta en las preferencias para usarla en el consumo
                Preferences.Set("RecetaSeleccionada", receta.Id);
                await DisplayAlert("Éxito", $"Receta '{receta.Nombre}' lista para usar", "OK");
                await Shell.Current.GoToAsync("///consumo");
            }
        }
    }

    private async void OnEliminarReceta(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string recetaId)
        {
            bool confirmado = await DisplayAlert("Eliminar", "¿Estás seguro de eliminar esta receta?", "Sí", "No");
            if (confirmado)
            {
                bool eliminada = await _dataService.EliminarReceta(recetaId);
                if (eliminada)
                {
                    await DisplayAlert("Éxito", "Receta eliminada", "OK");
                    await CargarRecetas();
                }
            }
        }
    }
}

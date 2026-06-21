using Nutri_TEC.Models;
using Nutri_TEC.Services;
using Microsoft.Maui.Controls.Shapes;

namespace Nutri_TEC.Views;

public partial class RegistroConsumoPage : ContentPage
{
    private readonly IDataService _dataService;
    private List<Producto> _productosDisponibles;
    private List<ProductoConsumo> _productosActuales;
    private string _usuarioId;

    public RegistroConsumoPage(IDataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;
        _productosActuales = new List<ProductoConsumo>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _usuarioId = Preferences.Get("UsuarioActual", "");
        await CargarProductos();
    }

    private async Task CargarProductos()
    {
        _productosDisponibles = await _dataService.ObtenerProductos();
        if (_productosDisponibles.Count == 0)
        {
            await _dataService.InicializarProductosDefault();
            _productosDisponibles = await _dataService.ObtenerProductos();
        }
    }

    private void OnSearchButtonPressed(object sender, EventArgs e)
    {
        string query = SearchBar.Text?.Trim();
        if (string.IsNullOrEmpty(query))
        {
            ResultadosContainer.Children.Clear();
            ResultadosContainer.Add(new Label { Text = "Busca un producto para comenzar", TextColor = Color.FromArgb("#999"), Margin = new Thickness(0, 20, 0, 0), HorizontalOptions = LayoutOptions.Center });
            return;
        }

        var resultados = _productosDisponibles
            .Where(p => p.Descripcion.Contains(query, StringComparison.OrdinalIgnoreCase) || p.CodigoBarras.Contains(query))
            .ToList();

        ResultadosContainer.Children.Clear();

        if (resultados.Count == 0)
        {
            ResultadosContainer.Add(new Label { Text = "No se encontraron productos", TextColor = Color.FromArgb("#999"), HorizontalOptions = LayoutOptions.Center });
            return;
        }

        foreach (var producto in resultados)
        {
            var frame = ConstruirFrameProducto(producto);
            ResultadosContainer.Add(frame);
        }
    }

    private Border ConstruirFrameProducto(Producto producto)
    {
        var frame = new Border
        {
            Stroke = Color.FromArgb("#ddd"),
            StrokeThickness = 1,
            Padding = 12,
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            Margin = new Thickness(0, 5, 0, 5)
        };

        var grid = new Grid
        {
            ColumnSpacing = 10
        };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var infoStack = new VerticalStackLayout
        {
            Spacing = 5
        };
        infoStack.Add(new Label { Text = producto.Descripcion, FontAttributes = FontAttributes.Bold, FontSize = 12 });
        infoStack.Add(new Label { Text = $"{producto.Energia} kcal | Código: {producto.CodigoBarras}", FontSize = 11, TextColor = Color.FromArgb("#666") });

        var entry = new Entry
        {
            Placeholder = "100",
            Keyboard = Keyboard.Numeric,
            Text = "100",
            WidthRequest = 70
        };
        entry.ClassId = producto.Id;

        var button = new Button
        {
            Text = "Agregar",
            Background = Color.FromArgb("#2ecc71"),
            TextColor = Colors.White,
            Padding = 10,
            FontSize = 12,
            CommandParameter = producto
        };
        button.Clicked += async (s, e) => await OnAgregarProducto(producto, entry);

        grid.Add(infoStack, 0, 0);
        grid.Add(button, 1, 0);

        frame.Content = grid;
        return frame;
    }

    private async Task OnAgregarProducto(Producto producto, Entry cantidadEntry)
    {
        if (!int.TryParse(cantidadEntry.Text, out int cantidad) || cantidad <= 0)
        {
            await DisplayAlert("Error", "Ingresa una cantidad válida", "OK");
            return;
        }

        if (TiempoComidaPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Error", "Selecciona un tiempo de comida", "OK");
            return;
        }

        var productoExistente = _productosActuales.FirstOrDefault(p => p.ProductoId == producto.Id);
        if (productoExistente != null)
        {
            productoExistente.Cantidad += cantidad;
        }
        else
        {
            _productosActuales.Add(new ProductoConsumo
            {
                ProductoId = producto.Id,
                Cantidad = cantidad,
                Producto = producto
            });
        }

        ActualizarVista();
        await DisplayAlert("Éxito", $"{producto.Descripcion} agregado", "OK");
    }

    private void OnEliminarProducto(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is ProductoConsumo pc)
        {
            _productosActuales.Remove(pc);
            ActualizarVista();
        }
    }

    private void ActualizarVista()
    {
        ProductosAgregarosView.ItemsSource = null;
        ProductosAgregarosView.ItemsSource = _productosActuales;

        double calorias = 0, proteinas = 0, carbos = 0;
        foreach (var pc in _productosActuales)
        {
            calorias += (pc.Producto.Energia * pc.Cantidad) / 100;
            proteinas += (pc.Producto.Proteina * pc.Cantidad) / 100;
            carbos += (pc.Producto.Carbohidratos * pc.Cantidad) / 100;
        }

        CaloriasLabel.Text = calorias.ToString("F0");
        ProteinasLabel.Text = proteinas.ToString("F1") + "g";
        CarbosLabel.Text = carbos.ToString("F1") + "g";
    }

    private async void OnGuardarConsumo(object sender, EventArgs e)
    {
        if (TiempoComidaPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Error", "Selecciona un tiempo de comida", "OK");
            return;
        }

        if (_productosActuales.Count == 0)
        {
            await DisplayAlert("Error", "Agrega al menos un producto", "OK");
            return;
        }

        string tiempoComida = (string)TiempoComidaPicker.SelectedItem;
        var consumo = new Consumo
        {
            UsuarioId = _usuarioId,
            TiempoComida = tiempoComida,
            Productos = _productosActuales
        };

        bool guardado = await _dataService.AgregarConsumo(consumo);
        if (guardado)
        {
            await DisplayAlert("Éxito", "Consumo guardado correctamente", "OK");
            LimpiarFormulario();
        }
    }

    private async void OnCrearReceta(object sender, EventArgs e)
    {
        if (_productosActuales.Count == 0)
        {
            await DisplayAlert("Error", "Agrega productos a la receta primero", "OK");
            return;
        }

        await Shell.Current.GoToAsync($"crear-receta?productos={string.Join(',', _productosActuales.Select(p => p.ProductoId))}");
    }

    private void LimpiarFormulario()
    {
        _productosActuales.Clear();
        TiempoComidaPicker.SelectedIndex = -1;
        SearchBar.Text = "";
        ActualizarVista();
    }
}

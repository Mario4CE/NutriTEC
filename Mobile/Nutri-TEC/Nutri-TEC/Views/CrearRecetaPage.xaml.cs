using Nutri_TEC.Models;
using Nutri_TEC.Services;

namespace Nutri_TEC.Views;

[QueryProperty(nameof(ProductosIds), "productos")]
public partial class CrearRecetaPage : ContentPage
{
    private readonly IDataService _dataService;
    private List<Producto> _productosDisponibles;
    private List<ProductoReceta> _productosReceta;
    private string _usuarioId;
    private string _productosIds;

    public string ProductosIds
    {
        get => _productosIds;
        set => _productosIds = value;
    }

    public CrearRecetaPage(IDataService dataService)
    {
        InitializeComponent();
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _productosDisponibles = new List<Producto>();
        _productosReceta = new List<ProductoReceta>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            _usuarioId = Preferences.Get("UsuarioActual", "");
            if (string.IsNullOrEmpty(_usuarioId))
            {
                await DisplayAlert("Error", "No hay usuario activo", "OK");
                await Shell.Current.GoToAsync("///consumo");
                return;
            }

            await CargarProductos();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en OnAppearing: {ex}");
            await DisplayAlert("Error", $"Error al cargar productos: {ex.Message}", "OK");
        }
    }

    private async Task CargarProductos()
    {
        try
        {
            _productosDisponibles = await _dataService.ObtenerProductos();
            if (_productosDisponibles == null)
            {
                _productosDisponibles = new List<Producto>();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en CargarProductos: {ex}");
            _productosDisponibles = new List<Producto>();
            await DisplayAlert("Error", "No se pudieron cargar los productos", "OK");
        }
    }

    private void OnSearchProducto(object sender, EventArgs e)
    {
        try
        {
            string query = SearchBar.Text?.Trim();
            if (string.IsNullOrEmpty(query))
            {
                ResultadosView.ItemsSource = null;
                return;
            }

            if (_productosDisponibles == null || _productosDisponibles.Count == 0)
            {
                ResultadosView.ItemsSource = new List<Producto>();
                return;
            }

            var resultados = _productosDisponibles
                .Where(p => !string.IsNullOrEmpty(p.Descripcion) &&
                           p.Descripcion.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

            ResultadosView.ItemsSource = resultados;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en OnSearchProducto: {ex}");
            ResultadosView.ItemsSource = new List<Producto>();
        }
    }

    private void OnSeleccionarProducto(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e?.CurrentSelection?.FirstOrDefault() is Producto producto && producto != null)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    string cantidad = await DisplayPromptAsync("Cantidad", "¿Cuántos gramos?", initialValue: "100", keyboard: Keyboard.Numeric);
                    if (!string.IsNullOrEmpty(cantidad) && int.TryParse(cantidad, out int cant) && cant > 0)
                    {
                        var productoExistente = _productosReceta.FirstOrDefault(p => p.ProductoId == producto.Id);
                        if (productoExistente != null)
                        {
                            productoExistente.Cantidad += cant;
                        }
                        else
                        {
                            _productosReceta.Add(new ProductoReceta
                            {
                                ProductoId = producto.Id,
                                Cantidad = cant,
                                Producto = producto
                            });
                        }
                        ActualizarVista();
                    }
                    ResultadosView.SelectedItem = null;
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en OnSeleccionarProducto: {ex}");
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Error", "No se pudo agregar el producto", "OK");
            });
        }
    }

    private void OnEliminarProductoReceta(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button button && button.CommandParameter is ProductoReceta pr)
            {
                _productosReceta.Remove(pr);
                ActualizarVista();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en OnEliminarProductoReceta: {ex}");
        }
    }

    private void ActualizarVista()
    {
        try
        {
            ProductosRecetaView.ItemsSource = null;
            ProductosRecetaView.ItemsSource = _productosReceta;

            double calorias = 0, proteinas = 0, grasas = 0, carbos = 0;
            foreach (var pr in _productosReceta)
            {
                if (pr?.Producto != null)
                {
                    calorias += (pr.Producto.Energia * pr.Cantidad) / 100;
                    proteinas += (pr.Producto.Proteina * pr.Cantidad) / 100;
                    grasas += (pr.Producto.Grasa * pr.Cantidad) / 100;
                    carbos += (pr.Producto.Carbohidratos * pr.Cantidad) / 100;
                }
            }

            CaloriasLabel.Text = calorias.ToString("F0");
            ProteinasLabel.Text = proteinas.ToString("F1");
            GrasasLabel.Text = grasas.ToString("F1");
            CarbosLabel.Text = carbos.ToString("F1");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en ActualizarVista: {ex}");
        }
    }

    private async void OnGuardarReceta(object sender, EventArgs e)
    {
        try
        {
            string nombre = NombreRecetaEntry.Text?.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                await DisplayAlert("Error", "Ingresa un nombre para la receta", "OK");
                return;
            }

            if (_productosReceta.Count == 0)
            {
                await DisplayAlert("Error", "Agrega al menos un producto", "OK");
                return;
            }

            double calorias = 0, proteinas = 0, grasas = 0, carbos = 0, sodio = 0;
            foreach (var pr in _productosReceta)
            {
                if (pr?.Producto != null)
                {
                    calorias += (pr.Producto.Energia * pr.Cantidad) / 100;
                    proteinas += (pr.Producto.Proteina * pr.Cantidad) / 100;
                    grasas += (pr.Producto.Grasa * pr.Cantidad) / 100;
                    carbos += (pr.Producto.Carbohidratos * pr.Cantidad) / 100;
                    sodio += (pr.Producto.Sodio * pr.Cantidad) / 100;
                }
            }

            var receta = new Receta
            {
                UsuarioId = _usuarioId,
                Nombre = nombre,
                Descripcion = DescripcionEditor.Text ?? "",
                Productos = _productosReceta,
                TotalCalorias = calorias,
                TotalProteinas = proteinas,
                TotalGrasas = grasas,
                TotalCarbohidratos = carbos,
                TotalSodio = sodio
            };

            bool guardada = await _dataService.GuardarReceta(receta);
            if (guardada)
            {
                await DisplayAlert("Éxito", $"Receta '{nombre}' guardada correctamente", "OK");
                await Shell.Current.GoToAsync("///consumo");
            }
            else
            {
                await DisplayAlert("Error", "No se pudo guardar la receta", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en OnGuardarReceta: {ex}");
            await DisplayAlert("Error", $"Error al guardar receta: {ex.Message}", "OK");
        }
    }

    private async void OnCancelar(object sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync("///consumo");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en OnCancelar: {ex}");
            await DisplayAlert("Error", "No se pudo volver", "OK");
        }
    }
}

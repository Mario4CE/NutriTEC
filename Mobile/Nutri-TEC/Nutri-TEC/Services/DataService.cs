using Nutri_TEC.Models;
using System.Text.Json;
using System.Diagnostics;

namespace Nutri_TEC.Services
{
    public interface IDataService
    {
        // Usuarios
        Task<bool> RegistrarUsuario(Usuario usuario);
        Task<Usuario> ValidarLogin(string email, string contraseña);
        Task<Usuario> ObtenerUsuario(string usuarioId);
        Task InicializarUsuariosDefault();

        // Productos
        Task<List<Producto>> ObtenerProductos();
        Task<List<Producto>> BuscarProductos(string query);
        Task<Producto> ObtenerProducto(string productoId);
        Task InicializarProductosDefault();

        // Consumos
        Task<bool> AgregarConsumo(Consumo consumo);
        Task<List<Consumo>> ObtenerConsumosDelDia(string usuarioId);
        Task<List<Consumo>> ObtenerConsumosPorFecha(string usuarioId, DateTime fecha);
        Task<bool> EliminarConsumo(string consumoId);

        // Recetas
        Task<bool> GuardarReceta(Receta receta);
        Task<List<Receta>> ObtenerRecetasDelUsuario(string usuarioId);
        Task<Receta> ObtenerReceta(string recetaId);
        Task<bool> EliminarReceta(string recetaId);
        Task InicializarRecetasDefault();
    }

    public class DataService : IDataService
    {
        private const string USUARIOS_FILE = "usuarios.json";
        private const string PRODUCTOS_FILE = "productos.json";
        private const string CONSUMOS_FILE = "consumos.json";
        private const string RECETAS_FILE = "recetas.json";
        private readonly string _dataPath;

        public DataService()
        {
            _dataPath = FileSystem.AppDataDirectory;
        }

        private string GetFilePath(string filename) => Path.Combine(_dataPath, filename);

        private async Task<T> LeerArchivo<T>(string filename, T valorDefault = default)
        {
            try
            {
                string filePath = GetFilePath(filename);
                if (!File.Exists(filePath))
                    return valorDefault;

                string json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
                return valorDefault;
            }
        }

        private async Task GuardarArchivo<T>(string filename, T datos)
        {
            try
            {
                string filePath = GetFilePath(filename);
                string json = JsonSerializer.Serialize(datos, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error guardando {filename}: {ex.Message}");
            }
        }

        #region Usuarios
        public async Task<bool> RegistrarUsuario(Usuario usuario)
        {
            var usuarios = await LeerArchivo<List<Usuario>>(USUARIOS_FILE, new List<Usuario>());

            if (usuarios.Any(u => u.Email == usuario.Email))
                return false;

            usuario.Id = Guid.NewGuid().ToString();
            usuarios.Add(usuario);
            await GuardarArchivo(USUARIOS_FILE, usuarios);
            return true;
        }

        public async Task InicializarUsuariosDefault()
        {
            var usuarios = await LeerArchivo<List<Usuario>>(USUARIOS_FILE, new List<Usuario>());
            if (usuarios.Count > 0)
                return;

            var usuariosDefault = new List<Usuario>
            {
                new() 
                { 
                    Id = Guid.NewGuid().ToString(),
                    Email = "juan@test.com", 
                    Nombre = "Juan Test", 
                    Contraseña = "123456",
                    Peso = 75,
                    Estatura = 175,
                    Edad = 30
                }
            };

            await GuardarArchivo(USUARIOS_FILE, usuariosDefault);
        }

        public async Task<Usuario> ValidarLogin(string email, string contraseña)
        {
            var usuarios = await LeerArchivo<List<Usuario>>(USUARIOS_FILE, new List<Usuario>());
            return usuarios.FirstOrDefault(u => u.Email == email && u.Contraseña == contraseña);
        }

        public async Task<Usuario> ObtenerUsuario(string usuarioId)
        {
            var usuarios = await LeerArchivo<List<Usuario>>(USUARIOS_FILE, new List<Usuario>());
            return usuarios.FirstOrDefault(u => u.Id == usuarioId);
        }
        #endregion

        #region Productos
        public async Task<List<Producto>> ObtenerProductos()
        {
            var productos = await LeerArchivo<List<Producto>>(PRODUCTOS_FILE, new List<Producto>());
            return productos ?? new List<Producto>();
        }

        public async Task<List<Producto>> BuscarProductos(string query)
        {
            var productos = await ObtenerProductos();
            var queryLower = query.ToLower();
            return productos.Where(p => 
                p.Descripcion.ToLower().Contains(queryLower) || 
                p.CodigoBarras.Contains(query)).ToList();
        }

        public async Task<Producto> ObtenerProducto(string productoId)
        {
            var productos = await ObtenerProductos();
            return productos.FirstOrDefault(p => p.Id == productoId);
        }

        public async Task InicializarProductosDefault()
        {
            var productos = await ObtenerProductos();
            if (productos.Count > 0)
                return;

            var productosDefault = new List<Producto>
            {
                new() { CodigoBarras = "0001", Descripcion = "Arroz blanco (100g)", Energia = 206, Proteina = 4, Grasa = 0.3, Carbohidratos = 45 },
                new() { CodigoBarras = "0002", Descripcion = "Pechuga de pollo (100g)", Energia = 165, Proteina = 31, Grasa = 3.6, Carbohidratos = 0 },
                new() { CodigoBarras = "0003", Descripcion = "Manzana roja (100g)", Energia = 95, Proteina = 0.3, Grasa = 0.2, Carbohidratos = 25 },
                new() { CodigoBarras = "0004", Descripcion = "Plátano (100g)", Energia = 105, Proteina = 1.1, Grasa = 0.3, Carbohidratos = 27 },
                new() { CodigoBarras = "0005", Descripcion = "Huevo (100g)", Energia = 78, Proteina = 13, Grasa = 5.5, Carbohidratos = 0.6 },
                new() { CodigoBarras = "0006", Descripcion = "Pan blanco (100g)", Energia = 80, Proteina = 2.7, Grasa = 1.1, Carbohidratos = 14.6 },
                new() { CodigoBarras = "0007", Descripcion = "Leche entera (100ml)", Energia = 80, Proteina = 3.2, Grasa = 4.7, Carbohidratos = 4.8 },
                new() { CodigoBarras = "0008", Descripcion = "Brócoli (100g)", Energia = 55, Proteina = 3.7, Grasa = 0.4, Carbohidratos = 11.2 },
            };

            await GuardarArchivo(PRODUCTOS_FILE, productosDefault);
        }
        #endregion

        #region Consumos
        public async Task<bool> AgregarConsumo(Consumo consumo)
        {
            var consumos = await LeerArchivo<List<Consumo>>(CONSUMOS_FILE, new List<Consumo>());
            consumos.Add(consumo);
            await GuardarArchivo(CONSUMOS_FILE, consumos);
            return true;
        }

        public async Task<List<Consumo>> ObtenerConsumosDelDia(string usuarioId)
        {
            var consumos = await LeerArchivo<List<Consumo>>(CONSUMOS_FILE, new List<Consumo>());
            var hoy = DateTime.Now.Date;
            return consumos.Where(c => c.UsuarioId == usuarioId && c.Fecha.Date == hoy).ToList();
        }

        public async Task<List<Consumo>> ObtenerConsumosPorFecha(string usuarioId, DateTime fecha)
        {
            var consumos = await LeerArchivo<List<Consumo>>(CONSUMOS_FILE, new List<Consumo>());
            var fechaDate = fecha.Date;
            return consumos.Where(c => c.UsuarioId == usuarioId && c.Fecha.Date == fechaDate).ToList();
        }

        public async Task<bool> EliminarConsumo(string consumoId)
        {
            var consumos = await LeerArchivo<List<Consumo>>(CONSUMOS_FILE, new List<Consumo>());
            var consumo = consumos.FirstOrDefault(c => c.Id == consumoId);
            if (consumo == null)
                return false;

            consumos.Remove(consumo);
            await GuardarArchivo(CONSUMOS_FILE, consumos);
            return true;
        }
        #endregion

        #region Recetas
        public async Task<bool> GuardarReceta(Receta receta)
        {
            var recetas = await LeerArchivo<List<Receta>>(RECETAS_FILE, new List<Receta>());
            recetas.Add(receta);
            await GuardarArchivo(RECETAS_FILE, recetas);
            return true;
        }

        public async Task<List<Receta>> ObtenerRecetasDelUsuario(string usuarioId)
        {
            var recetas = await LeerArchivo<List<Receta>>(RECETAS_FILE, new List<Receta>());
            return recetas.Where(r => r.UsuarioId == usuarioId).ToList();
        }

        public async Task<Receta> ObtenerReceta(string recetaId)
        {
            var recetas = await LeerArchivo<List<Receta>>(RECETAS_FILE, new List<Receta>());
            return recetas.FirstOrDefault(r => r.Id == recetaId);
        }

        public async Task<bool> EliminarReceta(string recetaId)
        {
            var recetas = await LeerArchivo<List<Receta>>(RECETAS_FILE, new List<Receta>());
            var receta = recetas.FirstOrDefault(r => r.Id == recetaId);
            if (receta == null)
                return false;

            recetas.Remove(receta);
            await GuardarArchivo(RECETAS_FILE, recetas);
            return true;
        }

        public async Task InicializarRecetasDefault()
        {
            var recetas = await LeerArchivo<List<Receta>>(RECETAS_FILE, new List<Receta>());
            if (recetas.Count > 0)
                return;

            // Obtener usuario de prueba
            var usuarios = await LeerArchivo<List<Usuario>>(USUARIOS_FILE, new List<Usuario>());
            var usuarioTest = usuarios.FirstOrDefault();
            if (usuarioTest == null)
                return;

            // Obtener productos
            var productos = await ObtenerProductos();

            var recetasDefault = new List<Receta>();

            // Receta 1: Desayuno Proteico
            var desayunoProteico = new Receta
            {
                UsuarioId = usuarioTest.Id,
                Nombre = "Desayuno Proteico",
                Descripcion = "Pan con huevo y jugo de naranja",
                Productos = new List<ProductoReceta>()
            };

            var pan = productos.FirstOrDefault(p => p.CodigoBarras == "0006");
            var huevo = productos.FirstOrDefault(p => p.CodigoBarras == "0005");

            if (pan != null)
            {
                desayunoProteico.Productos.Add(new ProductoReceta { ProductoId = pan.Id, Cantidad = 100, Producto = pan });
                desayunoProteico.TotalCalorias += pan.Energia;
                desayunoProteico.TotalProteinas += pan.Proteina;
                desayunoProteico.TotalGrasas += pan.Grasa;
                desayunoProteico.TotalCarbohidratos += pan.Carbohidratos;
            }

            if (huevo != null)
            {
                desayunoProteico.Productos.Add(new ProductoReceta { ProductoId = huevo.Id, Cantidad = 100, Producto = huevo });
                desayunoProteico.TotalCalorias += huevo.Energia;
                desayunoProteico.TotalProteinas += huevo.Proteina;
                desayunoProteico.TotalGrasas += huevo.Grasa;
                desayunoProteico.TotalCarbohidratos += huevo.Carbohidratos;
            }

            recetasDefault.Add(desayunoProteico);

            // Receta 2: Almuerzo Balanceado
            var almuerzoBalanceado = new Receta
            {
                UsuarioId = usuarioTest.Id,
                Nombre = "Almuerzo Balanceado",
                Descripcion = "Pechuga de pollo con arroz y brócoli",
                Productos = new List<ProductoReceta>()
            };

            var pollo = productos.FirstOrDefault(p => p.CodigoBarras == "0002");
            var arroz = productos.FirstOrDefault(p => p.CodigoBarras == "0001");
            var brocoli = productos.FirstOrDefault(p => p.CodigoBarras == "0008");

            if (pollo != null)
            {
                almuerzoBalanceado.Productos.Add(new ProductoReceta { ProductoId = pollo.Id, Cantidad = 150, Producto = pollo });
                almuerzoBalanceado.TotalCalorias += pollo.Energia * 1.5;
                almuerzoBalanceado.TotalProteinas += pollo.Proteina * 1.5;
                almuerzoBalanceado.TotalGrasas += pollo.Grasa * 1.5;
                almuerzoBalanceado.TotalCarbohidratos += pollo.Carbohidratos * 1.5;
            }

            if (arroz != null)
            {
                almuerzoBalanceado.Productos.Add(new ProductoReceta { ProductoId = arroz.Id, Cantidad = 100, Producto = arroz });
                almuerzoBalanceado.TotalCalorias += arroz.Energia;
                almuerzoBalanceado.TotalProteinas += arroz.Proteina;
                almuerzoBalanceado.TotalGrasas += arroz.Grasa;
                almuerzoBalanceado.TotalCarbohidratos += arroz.Carbohidratos;
            }

            if (brocoli != null)
            {
                almuerzoBalanceado.Productos.Add(new ProductoReceta { ProductoId = brocoli.Id, Cantidad = 100, Producto = brocoli });
                almuerzoBalanceado.TotalCalorias += brocoli.Energia;
                almuerzoBalanceado.TotalProteinas += brocoli.Proteina;
                almuerzoBalanceado.TotalGrasas += brocoli.Grasa;
                almuerzoBalanceado.TotalCarbohidratos += brocoli.Carbohidratos;
            }

            recetasDefault.Add(almuerzoBalanceado);

            // Receta 3: Snack Saludable
            var snackSaludable = new Receta
            {
                UsuarioId = usuarioTest.Id,
                Nombre = "Snack Saludable",
                Descripcion = "Plátano y manzana",
                Productos = new List<ProductoReceta>()
            };

            var platano = productos.FirstOrDefault(p => p.CodigoBarras == "0004");
            var manzana = productos.FirstOrDefault(p => p.CodigoBarras == "0003");

            if (platano != null)
            {
                snackSaludable.Productos.Add(new ProductoReceta { ProductoId = platano.Id, Cantidad = 100, Producto = platano });
                snackSaludable.TotalCalorias += platano.Energia;
                snackSaludable.TotalProteinas += platano.Proteina;
                snackSaludable.TotalGrasas += platano.Grasa;
                snackSaludable.TotalCarbohidratos += platano.Carbohidratos;
            }

            if (manzana != null)
            {
                snackSaludable.Productos.Add(new ProductoReceta { ProductoId = manzana.Id, Cantidad = 100, Producto = manzana });
                snackSaludable.TotalCalorias += manzana.Energia;
                snackSaludable.TotalProteinas += manzana.Proteina;
                snackSaludable.TotalGrasas += manzana.Grasa;
                snackSaludable.TotalCarbohidratos += manzana.Carbohidratos;
            }

            recetasDefault.Add(snackSaludable);

            await GuardarArchivo(RECETAS_FILE, recetasDefault);
        }
        #endregion
    }
}

namespace NutriTECAPI.Models
{
    /// <summary>
    /// Modelo de Producto/Alimento
    /// </summary>
    public class Producto
    {
        public int Id { get; set; }

        public string CodigoBarras { get; set; }

        public string Descripcion { get; set; }

        public string Porcion { get; set; }

        public decimal Energia { get; set; }

        public decimal Grasa { get; set; }

        public decimal Sodio { get; set; }

        public decimal Carbohidratos { get; set; }

        public decimal Proteina { get; set; }

        public string? Vitaminas { get; set; }

        public decimal Calcio { get; set; }

        public decimal Hierro { get; set; }

        public string Estado { get; set; } = "pendiente"; // pendiente, aprobado, rechazado

        public int? UsuarioId { get; set; } // Quien lo agregó

        public DateTime FechaCreacion { get; set; }

        // Relaciones
        public Usuario? Usuario { get; set; }

        public ICollection<ConsumoProducto> ConsumoProductos { get; set; } = new List<ConsumoProducto>();

        public ICollection<RecetaProducto> RecetaProductos { get; set; } = new List<RecetaProducto>();
    }
}

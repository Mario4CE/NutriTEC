namespace NutriTECAPI.Models
{
    /// <summary>
    /// Modelo de Receta
    /// </summary>
    public class Receta
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public string Nombre { get; set; }

        public DateTime FechaCreacion { get; set; }

        // Relaciones
        public Usuario? Usuario { get; set; }

        public ICollection<RecetaProducto> RecetaProductos { get; set; } = new List<RecetaProducto>();
    }

    /// <summary>
    /// Tabla asociativa: Receta - Producto
    /// </summary>
    public class RecetaProducto
    {
        public int Id { get; set; }

        public int RecetaId { get; set; }

        public int ProductoId { get; set; }

        public decimal Cantidad { get; set; } // Cantidad de porciones

        // Relaciones
        public Receta? Receta { get; set; }

        public Producto? Producto { get; set; }
    }
}

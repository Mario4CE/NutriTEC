namespace Nutri_TEC.Models
{
    public class ProductoReceta
    {
        public string ProductoId { get; set; }
        public int Cantidad { get; set; } // en gramos
        public Producto Producto { get; set; }
    }

    public class Receta
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public List<ProductoReceta> Productos { get; set; } = new();
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Nutrientes totales
        public double TotalCalorias { get; set; }
        public double TotalProteinas { get; set; }
        public double TotalGrasas { get; set; }
        public double TotalCarbohidratos { get; set; }
        public double TotalSodio { get; set; }
    }
}

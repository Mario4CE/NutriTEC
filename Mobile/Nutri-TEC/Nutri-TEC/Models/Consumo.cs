namespace Nutri_TEC.Models
{
    public class ProductoConsumo
    {
        public string ProductoId { get; set; }
        public int Cantidad { get; set; } // en gramos
        public Producto Producto { get; set; }
    }

    public class Consumo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UsuarioId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string TiempoComida { get; set; } // Desayuno, Almuerzo, Cena, etc
        public List<ProductoConsumo> Productos { get; set; } = new();
    }
}

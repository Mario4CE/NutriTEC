namespace NutriTECAPI.Models
{
    /// <summary>
    /// Modelo de Registro de Consumo Diario
    /// </summary>
    public class Consumo
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public DateTime Fecha { get; set; }

        public string TiempoComida { get; set; } // desayuno, merienda-manana, almuerzo, merienda-tarde, cena

        // Relaciones
        public Usuario? Usuario { get; set; }

        public ICollection<ConsumoProducto> ConsumoProductos { get; set; } = new List<ConsumoProducto>();
    }

    /// <summary>
    /// Tabla asociativa: Consumo - Producto
    /// </summary>
    public class ConsumoProducto
    {
        public int Id { get; set; }

        public int ConsumoId { get; set; }

        public int ProductoId { get; set; }

        public int Cantidad { get; set; } // Cantidad de porciones

        // Relaciones
        public Consumo? Consumo { get; set; }

        public Producto? Producto { get; set; }
    }
}

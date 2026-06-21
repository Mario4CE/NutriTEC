namespace NutriTECAPI.DTOs
{
    /// <summary>
    /// DTO para Medida
    /// </summary>
    public class MedidaDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Cintura { get; set; }
        public decimal Cuello { get; set; }
        public decimal Caderas { get; set; }
        public decimal PorcentajeMusculo { get; set; }
        public decimal PorcentajeGrasa { get; set; }
    }

    /// <summary>
    /// DTO para crear/actualizar Medida
    /// </summary>
    public class CreateMedidaDTO
    {
        public decimal Cintura { get; set; }
        public decimal Cuello { get; set; }
        public decimal Caderas { get; set; }
        public decimal PorcentajeMusculo { get; set; }
        public decimal PorcentajeGrasa { get; set; }
    }

    /// <summary>
    /// DTO para Producto
    /// </summary>
    public class ProductoDTO
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
        public string Vitaminas { get; set; }
        public decimal Calcio { get; set; }
        public decimal Hierro { get; set; }
        public string Estado { get; set; }
    }

    /// <summary>
    /// DTO para crear Producto
    /// </summary>
    public class CreateProductoDTO
    {
        public string CodigoBarras { get; set; }
        public string Descripcion { get; set; }
        public string Porcion { get; set; }
        public decimal Energia { get; set; }
        public decimal Grasa { get; set; }
        public decimal Sodio { get; set; }
        public decimal Carbohidratos { get; set; }
        public decimal Proteina { get; set; }
        public string Vitaminas { get; set; }
        public decimal Calcio { get; set; }
        public decimal Hierro { get; set; }
    }

    /// <summary>
    /// DTO para Consumo
    /// </summary>
    public class ConsumoDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string TiempoComida { get; set; }
        public List<ConsumoProductoDTO> Productos { get; set; }
    }

    /// <summary>
    /// DTO para Consumo-Producto
    /// </summary>
    public class ConsumoProductoDTO
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }

    /// <summary>
    /// DTO para Receta
    /// </summary>
    public class RecetaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<RecetaProductoDTO> Productos { get; set; }
    }

    /// <summary>
    /// DTO para Receta-Producto
    /// </summary>
    public class RecetaProductoDTO
    {
        public int ProductoId { get; set; }
        public decimal Cantidad { get; set; }
    }

    /// <summary>
    /// DTO para crear Receta
    /// </summary>
    public class CreateRecetaDTO
    {
        public string Nombre { get; set; }
        public List<RecetaProductoDTO> Productos { get; set; }
    }
}

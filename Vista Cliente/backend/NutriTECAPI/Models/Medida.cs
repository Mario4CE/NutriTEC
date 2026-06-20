namespace NutriTECAPI.Models
{
    /// <summary>
    /// Modelo de Medidas corporales del usuario
    /// </summary>
    public class Medida
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Cintura { get; set; }

        public decimal Cuello { get; set; }

        public decimal Caderas { get; set; }

        public decimal PorcentajeMusculo { get; set; }

        public decimal PorcentajeGrasa { get; set; }

        // Relaciones
        public Usuario? Usuario { get; set; }
    }
}

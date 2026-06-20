namespace NutriTECAPI.Models
{
    /// <summary>
    /// Modelo de Usuario (Cliente/Paciente)
    /// </summary>
    public class Usuario
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Apellidos { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int Edad { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public decimal Peso { get; set; }

        public decimal IMC { get; set; }

        public string Pais { get; set; }

        public decimal Cintura { get; set; }

        public decimal Cuello { get; set; }

        public decimal Caderas { get; set; }

        public decimal PorcentajeMusculo { get; set; }

        public decimal PorcentajeGrasa { get; set; }

        public int CaloriasDiarias { get; set; }

        public string? Foto { get; set; }

        public DateTime FechaRegistro { get; set; }

        public int? NutricionistaId { get; set; }

        // Relaciones
        public ICollection<Medida> Medidas { get; set; } = new List<Medida>();

        public ICollection<Consumo> Consumos { get; set; } = new List<Consumo>();

        public ICollection<Receta> Recetas { get; set; } = new List<Receta>();
    }
}

namespace Nutri_TEC.Models
{
    public class Usuario
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Contraseña { get; set; }
        public int Peso { get; set; }
        public int Estatura { get; set; }
        public int Edad { get; set; }
        public int CaloriasDiarias { get; set; } = 2200;
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}

namespace NutriTECAPI.DTOs
{
    /// <summary>
    /// DTO para Login
    /// </summary>
    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// DTO para Registro de Usuario
    /// </summary>
    public class RegisterDTO
    {
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
    }

    /// <summary>
    /// DTO para respuesta de Usuario (sin contraseña)
    /// </summary>
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public int Edad { get; set; }
        public decimal Peso { get; set; }
        public decimal IMC { get; set; }
        public string Pais { get; set; }
        public decimal Cintura { get; set; }
        public decimal Cuello { get; set; }
        public decimal Caderas { get; set; }
        public decimal PorcentajeMusculo { get; set; }
        public decimal PorcentajeGrasa { get; set; }
        public int CaloriasDiarias { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    /// <summary>
    /// DTO para respuesta de autenticación
    /// </summary>
    public class AuthResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public UsuarioDTO Usuario { get; set; }
    }
}

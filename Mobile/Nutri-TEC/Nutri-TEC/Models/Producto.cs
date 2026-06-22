namespace Nutri_TEC.Models
{
    public class Producto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CodigoBarras { get; set; }
        public string Descripcion { get; set; }
        public string Porcion { get; set; } = "100g";
        public double Energia { get; set; } // kcal por 100g
        public double Proteina { get; set; } // gramos por 100g
        public double Grasa { get; set; } // gramos por 100g
        public double Carbohidratos { get; set; } // gramos por 100g
        public double Sodio { get; set; } // mg por 100g
        public string Vitaminas { get; set; }
        public double Calcio { get; set; } // mg
        public double Hierro { get; set; } // mg
        public string Estado { get; set; } = "aprobado";
    }
}

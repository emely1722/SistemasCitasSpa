using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.Models
{
    public class MetodoPago
    {
        [Key]
        public int IdMetodoPago { get; set; }

        [Required(ErrorMessage = "El nombre del método de pago es requerido")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "El nombre debe tener de 3 a 60 caracteres")]
        public required string Nombre { get; set; }

        [StringLength(150, ErrorMessage = "La descripción no puede exceder 150 caracteres")]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        public List<Cita> Citas { get; set; } = new();
    }
}

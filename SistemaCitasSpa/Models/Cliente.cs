using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(60, MinimumLength = 2,
            ErrorMessage = "El nombre debe tener de 2 a 60 caracteres")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(60, MinimumLength = 2,
            ErrorMessage = "El apellido debe tener de 2 a 60 caracteres")]
        public required string Apellido { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public required string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [StringLength(120, ErrorMessage = "El correo no puede exceder 120 caracteres")]
        public string? Correo { get; set; }

        public bool Activo { get; set; } = true;

        public List<Cita> Citas { get; set; } = new();
    }
}
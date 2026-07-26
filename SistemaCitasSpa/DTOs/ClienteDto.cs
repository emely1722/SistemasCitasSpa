using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.DTOs
{
    public class ClienteDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(60, MinimumLength = 2)]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(60, MinimumLength = 2)]
        public required string Apellido { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido")]
        [StringLength(20)]
        public required string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El correo no es válido")]
        [StringLength(120)]
        public string? Correo { get; set; }

        public bool Activo { get; set; } = true;
    }
}
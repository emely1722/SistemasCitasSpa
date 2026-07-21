using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Nombre completo es requerido")]
        [StringLength(120, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener de 3 a 120 caracteres")]
        public required string NombreCompleto { get; set; }

        [Required(ErrorMessage = "Nombre de usuario es requerido")]
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "El usuario debe tener de 3 a 50 caracteres")]
        public required string NombreUsuario { get; set; }

        [Required(ErrorMessage = "Correo es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo incorrecto")]
        [StringLength(120, ErrorMessage = "El correo no puede exceder 120 caracteres")]
        public required string Correo { get; set; }

        [Required(ErrorMessage = "Clave requerida")]
        [StringLength(255, ErrorMessage = "La clave no puede exceder 255 caracteres")]
        public required string ClaveHash { get; set; }

        public bool Activo { get; set; } = true;
    }
}
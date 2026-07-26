using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Nombre es requerido")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener de 3 a 100 caracteres")]
        public required string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El usuario debe tener de 3 a 50 caracteres")]
        public required string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [StringLength(120, ErrorMessage = "El correo no puede exceder 120 caracteres")]
        public required string Correo { get; set; }

        [Required(ErrorMessage = "La clave es requerida")]
        public required string ClaveHash { get; set; }

        public bool Activo { get; set; } = true;
    }
}
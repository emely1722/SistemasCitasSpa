using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.DTOs
{
    public class RegistroUsuarioDto
    {
        [Required(ErrorMessage = "El nombre completo es requerido")]
        [StringLength(100, MinimumLength = 3)]
        public required string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50, MinimumLength = 3)]
        public required string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El correo no es válido")]
        public required string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "La contraseña debe tener mínimo 8 caracteres")]
        public required string Clave { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.DTOs
{
    public class ActualizarUsuarioDto
    {
        [Required(ErrorMessage = "El nombre completo es requerido")]
        [StringLength(100)]
        public required string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50)]
        public required string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El correo no es válido")]
        public required string Correo { get; set; }

        public string? Clave { get; set; }

        public bool Activo { get; set; }
    }
}
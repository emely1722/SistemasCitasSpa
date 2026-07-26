using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.DTOs
{
    public class ActualizarUsuarioDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        public string Email { get; set; } = string.Empty;

        public string? Password { get; set; }

        public string Rol { get; set; } = "Usuario";
    }
}
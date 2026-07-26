using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El usuario o correo es requerido")]
        public required string UsuarioOCorreo { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        public required string Clave { get; set; }
    }
}
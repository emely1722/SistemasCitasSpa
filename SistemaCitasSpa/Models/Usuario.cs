using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemasCitasSpa.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(120)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(120)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string ClaveHash { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }
}

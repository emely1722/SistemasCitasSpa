using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.DTOs
{
    public class CategoriaDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(80, MinimumLength = 3)]
        public required string Nombre { get; set; }

        [StringLength(200)]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;
    }
}
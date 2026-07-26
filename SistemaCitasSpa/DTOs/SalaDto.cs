using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.DTOs
{
    public class SalaDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(80)]
        public required string Nombre { get; set; }

        [StringLength(200)]
        public string? Descripcion { get; set; }

        public bool Activa { get; set; } = true;
    }
}
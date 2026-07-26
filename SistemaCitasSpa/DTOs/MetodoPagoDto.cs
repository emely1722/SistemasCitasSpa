using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.DTOs
{
    public class MetodoPagoDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(60)]
        public required string Nombre { get; set; }

        [StringLength(150)]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;
    }
}
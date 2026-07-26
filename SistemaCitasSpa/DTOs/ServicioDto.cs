using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.DTOs
{
    public class ServicioDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public required string Nombre { get; set; }

        [StringLength(200)]
        public string? Descripcion { get; set; }

        [Range(typeof(decimal), "1", "999999",
            ErrorMessage = "El precio debe ser mayor que cero")]
        public decimal Precio { get; set; }

        [Range(1, 480, ErrorMessage = "La duración no es válida")]
        public int DuracionMinutos { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Debe seleccionar una categoría")]
        public int IdCategoria { get; set; }

        public bool Activo { get; set; } = true;
    }
}
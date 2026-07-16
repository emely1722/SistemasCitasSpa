using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.Models
{
    public class Servicio
    {
        public int IdServicio { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría")]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "El nombre del servicio es requerido")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener de 3 a 100 caracteres")]
        public required string Nombre { get; set; }

        [StringLength(300,
            ErrorMessage = "La descripción no puede exceder 300 caracteres")]
        public string? Descripcion { get; set; }

        [Range(typeof(decimal), "0.01", "99999999.99",
            ErrorMessage = "El precio debe ser mayor que cero")]
        public decimal Precio { get; set; }

        [Range(1, 480,
            ErrorMessage = "La duración debe estar entre 1 y 480 minutos")]
        public int DuracionMinutos { get; set; }

        public bool Activo { get; set; } = true;

        public CategoriaServicio? Categoria { get; set; }

        public List<Cita> Citas { get; set; } = new();
    }
}
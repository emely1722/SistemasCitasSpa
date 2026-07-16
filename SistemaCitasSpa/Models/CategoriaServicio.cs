using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.Models
{
    public class CategoriaServicio
    {
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es requerido")]
        [StringLength(80, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener de 3 a 80 caracteres")]
        public required string Nombre { get; set; }

        [StringLength(250,
            ErrorMessage = "La descripción no puede exceder 250 caracteres")]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        public List<Servicio> Servicios { get; set; } = new();
    }
}

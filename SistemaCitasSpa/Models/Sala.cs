using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.Models
{
    public class Sala
    {
        [Key]
        public int IdSala { get; set; }

        [Required(ErrorMessage = "El nombre de la sala es requerido")]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "El nombre debe tener de 2 a 80 caracteres")]
        public required string Nombre { get; set; }

        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
        public string? Descripcion { get; set; }

        public bool Activa { get; set; } = true;

        public List<Cita> Citas { get; set; } = new();
    }
}

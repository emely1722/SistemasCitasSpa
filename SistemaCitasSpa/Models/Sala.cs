using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.Models
{
    public class Sala
    {
        public int IdSala { get; set; }

        [Required(ErrorMessage = "El nombre de la sala es requerido")]
        [StringLength(80, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener de 3 a 80 caracteres")]
        public required string Nombre { get; set; }

        [StringLength(250,
            ErrorMessage = "La descripción no puede exceder 250 caracteres")]
        public string? Descripcion { get; set; }

        public bool Activa { get; set; } = true;

        public List<Cita> Citas { get; set; } = new();
    }
}
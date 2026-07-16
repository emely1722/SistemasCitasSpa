using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.Models
{
    public class HorarioTerapeuta
    {
        public int IdHorario { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un terapeuta")]
        public int IdTerapeuta { get; set; }

        [Required(ErrorMessage = "El día de la semana es requerido")]
        [StringLength(15,
            ErrorMessage = "El día no puede exceder 15 caracteres")]
        public required string DiaSemana { get; set; }

        public TimeOnly HoraInicio { get; set; }

        public TimeOnly HoraFin { get; set; }

        public bool Activo { get; set; } = true;

        public Terapeuta? Terapeuta { get; set; }
    }
}
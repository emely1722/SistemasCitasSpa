using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.DTOs
{
    public class HorarioTerapeutaDto
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Debe seleccionar un terapeuta")]
        public int IdTerapeuta { get; set; }

        [Required(ErrorMessage = "El día es requerido")]
        public required string DiaSemana { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        public bool Activo { get; set; } = true;
    }
}
using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.DTOs
{
    public class CrearCitaDto
    {
        [Required(ErrorMessage = "El cliente es obligatorio")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateOnly Fecha { get; set; }

        [Required(ErrorMessage = "La hora es obligatoria")]
        public TimeOnly Hora { get; set; }

        [Required(ErrorMessage = "El servicio es obligatorio")]
        public int IdServicio { get; set; }

        [Required(ErrorMessage = "El terapeuta es obligatorio")]
        public int IdTerapeuta { get; set; }

        [Required(ErrorMessage = "La sala es obligatoria")]
        public int IdSala { get; set; }

        public int? IdMetodoPago { get; set; }
    }
}
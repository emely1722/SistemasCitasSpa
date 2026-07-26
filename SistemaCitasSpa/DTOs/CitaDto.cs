using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.DTOs
{
    public class CitaDto
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Debe seleccionar un cliente")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La hora es requerida")]
        public TimeSpan Hora { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Debe seleccionar un servicio")]
        public int IdServicio { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Debe seleccionar un terapeuta")]
        public int IdTerapeuta { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Debe seleccionar una sala")]
        public int IdSala { get; set; }

        public int? IdMetodoPago { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace SistemasCitasSpa.Models
{
    public class Cita
    {
        public int IdCita { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un cliente")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "La fecha de la cita es requerida")]
        public DateOnly Fecha { get; set; }

        [Required(ErrorMessage = "La hora de la cita es requerida")]
        public TimeOnly Hora { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un servicio")]
        public int IdServicio { get; set; }

        [Range(1, 480,
            ErrorMessage = "La duración debe estar entre 1 y 480 minutos")]
        public int DuracionMinutos { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un terapeuta")]
        public int IdTerapeuta { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una sala")]
        public int IdSala { get; set; }

        public int? IdMetodoPago { get; set; }

        public DateTime FechaRegistro { get; set; }

        public Cliente? Cliente { get; set; }

        public Servicio? Servicio { get; set; }

        public Terapeuta? Terapeuta { get; set; }

        public Sala? Sala { get; set; }

        public MetodoPago? MetodoPago { get; set; }
    }
}
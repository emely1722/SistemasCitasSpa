using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemasCitasSpa.Models
{
    public class Cita
    {
        [Key]
        public int IdCita { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un cliente")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La hora es requerida")]
        public TimeSpan Hora { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un servicio")]
        public int IdServicio { get; set; }

        public int DuracionMinutos { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un terapeuta")]
        public int IdTerapeuta { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una sala")]
        public int IdSala { get; set; }

        public int? IdMetodoPago { get; set; }

        public Cliente? Cliente { get; set; }

        public Servicio? Servicio { get; set; }

        public Terapeuta? Terapeuta { get; set; }

        public Sala? Sala { get; set; }

        public MetodoPago? MetodoPago { get; set; }

        [NotMapped]
        public string Estado
        {
            get
            {
                DateTime inicio = Fecha.Date.Add(Hora);
                DateTime fin = inicio.AddMinutes(DuracionMinutos);

                if (DateTime.Now < inicio)
                    return "Vigente";

                if (DateTime.Now >= inicio && DateTime.Now < fin)
                    return "En proceso";

                return "Finalizado";
            }
        }

        [NotMapped]
        public string TiempoRestante
        {
            get
            {
                DateTime inicio = Fecha.Date.Add(Hora);

                if (DateTime.Now >= inicio)
                    return "La cita ya inició.";

                TimeSpan diferencia = inicio - DateTime.Now;

                return $"{diferencia.Days} día(s) y {diferencia.Hours} hora(s)";
            }
        }
    }
}

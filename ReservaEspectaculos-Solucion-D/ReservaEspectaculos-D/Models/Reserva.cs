using ReservaEspectaculos_D.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace ReservaEspectaculos_D.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        public Funcion Funcion { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = ErrorHelper.Data)]
        [Display(Name = "Fecha de alta")]
        public DateTime FechaAlta { get; set; } = DateTime.Now;

        public Cliente Cliente { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = ErrorHelper.NumRange)]
        [Display(Name = "Cantidad de butacas")]

        public int CantidadButacas { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        public int FuncionId { get; set; }

        public EstadoReserva EstadoReserva { get; set; }

    }
}

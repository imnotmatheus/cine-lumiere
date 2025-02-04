using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ReservaEspectaculos_D.Models.ViewModels
{
    public class ConfirmarReserva
    {
        public Pelicula Pelicula { get; set; }

        [Required]
        public DateOnly Fecha { get; set; }

        [Required]
        public TimeOnly Hora { get; set; }

        public Sala Sala { get; set; }

        [Required]
        [Display(Name = "Cantidad de Butacas")]
        public int CantidadButacas { get; set; }

        [Required]
        public int PeliculaId { get; set; }

        [Required]
        public int SalaId { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required]
        public int FuncionId { get; set; }
        [DisplayName("Precio Total")]
        public decimal PrecioTotal { get; set; }
    }
}

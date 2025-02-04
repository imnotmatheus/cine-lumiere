using Microsoft.IdentityModel.Tokens;
using ReservaEspectaculos_D.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ReservaEspectaculos_D.Models
{
    public class Funcion
    {
        public int Id { get; set; }

        [DataType(DataType.Date, ErrorMessage = ErrorHelper.Data)]
        public DateOnly Fecha { get; set; }

        [DataType(DataType.Time, ErrorMessage = ErrorHelper.Data)]
        public TimeOnly Hora { get; set; }

        [MaxLength(500, ErrorMessage = ErrorHelper.StrMax)]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        [Display(Name = "Butacas disponibles")]
        public int ButacasDisponibles { get; set; }

        public bool Confirmada { get; set; }

        public Pelicula Pelicula { get; set; }

        public Sala Sala { get; set; }

        public List<Reserva> Reservas { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        public int PeliculaId { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        public int SalaId { get; set; }

    }

}

using Microsoft.AspNetCore.Mvc;
using ReservaEspectaculos_D.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReservaEspectaculos_D.Models
{
    public class Sala
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [Range(1, 500, ErrorMessage = ErrorHelper.NumRange)]
        [Remote(action:"NumeroDisponible", controller:"Salas", AdditionalFields = nameof(Id))]
        public int Numero { get; set; }

        [Display(Name = "Tipo de sala")]
        public TipoSala TipoSala { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [Range(15, 1000, ErrorMessage = ErrorHelper.NumRange)]
        [Display(Name = "Capacidad")]
        public int CapacidadButacas { get; set; }

        public List<Funcion> Funciones { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        public int TipoSalaId { get; set; }
    }
}

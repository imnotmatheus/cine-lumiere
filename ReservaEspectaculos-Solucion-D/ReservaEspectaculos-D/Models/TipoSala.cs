using Microsoft.AspNetCore.Mvc;
using ReservaEspectaculos_D.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReservaEspectaculos_D.Models
{
    public class TipoSala
    {
        public int Id { get; set; }

        [MaxLength(20, ErrorMessage = ErrorHelper.StrMax)]
        [Remote(action:"NombreDisponible", controller:"TipoSalas", AdditionalFields = nameof(Id))]
        public string Nombre { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [Range(0, int.MaxValue, ErrorMessage = ErrorHelper.NumRange)]
        public decimal Precio { get; set; }

        public List<Sala> Salas { get; set; }    
    }
}

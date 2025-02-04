using Microsoft.AspNetCore.Mvc;
using ReservaEspectaculos_D.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReservaEspectaculos_D.Models
{
    public class Genero
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [StringLength(25, MinimumLength = 5, ErrorMessage = ErrorHelper.StrMaxMin)]
        [Remote(action: "NombreDisponible", controller: "Generos")]
        public string Nombre { get; set; }

        public List<Pelicula> Peliculas { get; set; }
    }
}

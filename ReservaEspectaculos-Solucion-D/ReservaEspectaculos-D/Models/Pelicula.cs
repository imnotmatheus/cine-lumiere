using Microsoft.AspNetCore.Mvc;
using ReservaEspectaculos_D.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReservaEspectaculos_D.Models
{
    public class Pelicula
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [MaxLength(170, ErrorMessage = ErrorHelper.StrMax)]
        [Display(Name = "Título")]
        [Remote(action:"TituloDisponible", controller:"Peliculas", AdditionalFields = nameof(Id))]
        public string Titulo { get; set; }

        [DataType(DataType.Date, ErrorMessage = ErrorHelper.Data)]
        [Display(Name = "Fecha de lanzamiento")]
        public DateOnly FechaLanzamiento { get; set; }

        [MaxLength(500, ErrorMessage = ErrorHelper.StrMax)]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        public List<Funcion> Funciones { get; set; }

        public Genero Genero { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [Display(Name = "Genero")]
        public int GeneroId { get; set; }

        public string PathCartel { get; set; }
    }
}

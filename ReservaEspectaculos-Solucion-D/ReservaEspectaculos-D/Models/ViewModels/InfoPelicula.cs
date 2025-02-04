using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ReservaEspectaculos_D.Models.ViewModels
{
    public class InfoPelicula
    {
        public int PeliculaId { get; set; }
        public string Titulo { get; set; }

        [Display(Name = "Lanzamiento")]
        public DateOnly FechaLanzamiento { get; set; }

        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        public IEnumerable<FuncionEnIndex> Funciones { get; set; }

        public Genero Genero { get; set; }

        public string PathCartel { get; set; }
    }
}

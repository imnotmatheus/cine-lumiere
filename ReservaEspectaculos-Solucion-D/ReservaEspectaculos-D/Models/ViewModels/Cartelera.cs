using System.Collections;
using System.Collections.Generic;

namespace ReservaEspectaculos_D.Models.ViewModels
{
    public class Cartelera
    {
        public IEnumerable<Pelicula> Peliculas { get; set; }
        public IEnumerable<Genero> Generos { get; set; }
    }
}

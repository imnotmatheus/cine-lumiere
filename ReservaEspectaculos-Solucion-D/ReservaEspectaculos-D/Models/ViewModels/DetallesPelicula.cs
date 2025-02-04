using System.Collections.Generic;
using System.ComponentModel;


namespace ReservaEspectaculos_D.Models.ViewModels
{
    public class DetallesPelicula
    {
        public Pelicula Pelicula { get; set; }

        [DisplayName("Recaudación del último mes")]
        public decimal RecaudacionTotal { get; set; }
        public List<Funcion> Funciones { get; set; } 
    }
}

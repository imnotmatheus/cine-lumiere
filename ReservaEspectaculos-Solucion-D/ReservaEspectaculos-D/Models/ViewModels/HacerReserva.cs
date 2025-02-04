using System.Collections.Generic;

namespace ReservaEspectaculos_D.Models.ViewModels
{
    public class HacerReserva
    {
        public int ClienteId { get; set; }
        public int PeliculaId { get; set; }
        public int CantidadButacas { get; set; }
        public int? FuncionSeleccionadaId { get; set; }
        public FuncionEnIndex FuncionSeleccionada { get; set; }
        public List<FuncionEnIndex> Funciones { get; set; }


    }
}

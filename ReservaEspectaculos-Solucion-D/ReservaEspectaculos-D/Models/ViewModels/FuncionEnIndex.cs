using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReservaEspectaculos_D.Models.ViewModels
{
    public class FuncionEnIndex
    {
        public Funcion Funcion { get; set; }
        public decimal Recaudacion {  get; set; }
        [Display(Name = "Butacas disponibles")]
        public int ButacasDisponibles { get; set; }
    }
}

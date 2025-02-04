using System.Collections.Generic;

namespace ReservaEspectaculos_D.Models
{
    public class Cliente : Persona {
        public List<Reserva> Reservas { get; set; }
    }
}

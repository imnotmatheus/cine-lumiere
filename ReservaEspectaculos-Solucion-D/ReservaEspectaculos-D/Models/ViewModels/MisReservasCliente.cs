using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace ReservaEspectaculos_D.Models.ViewModels
{
    public class MisReservasCliente
    {
        public IEnumerable<Reserva> ReservasVigentes {  get; set; }
        public IEnumerable<Reserva> ReservasPasadas { get; set; }
        public int ClienteId { get; set; }
    }
}

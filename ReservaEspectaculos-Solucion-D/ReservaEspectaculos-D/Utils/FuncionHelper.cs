using Microsoft.IdentityModel.Tokens;
using ReservaEspectaculos_D.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReservaEspectaculos_D.Utils
{
    public static class FuncionHelper
    {

        public static decimal calcularRecaudacion(Funcion f)
        {
            decimal cantButacas = 0;
            var reservas = f.Reservas;
            if (!f.Reservas.IsNullOrEmpty())
            {
                foreach (Reserva reserva in reservas)
                {
                    cantButacas += reserva.CantidadButacas;
                }
                return cantButacas * f.Sala.TipoSala.Precio;
            }
            return 0;


        }
    }
}

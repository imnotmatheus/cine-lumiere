using Microsoft.IdentityModel.Tokens;
using ReservaEspectaculos_D.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReservaEspectaculos_D.Utils
{
    public static class FuncionHelper
    {
        public const int DuracionFunciones = 3;


        public static int ButacasDisponibles(Funcion f)
        {
            var reservas = f.Reservas;


            if (reservas.IsNullOrEmpty())
            {
                return f.Sala.CapacidadButacas;
            }

            int cantidadReservadas = 0;

            foreach (Reserva reserva in f.Reservas)
            {
                cantidadReservadas += reserva.CantidadButacas;
            }
            return f.Sala.CapacidadButacas - cantidadReservadas;

        }



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


    


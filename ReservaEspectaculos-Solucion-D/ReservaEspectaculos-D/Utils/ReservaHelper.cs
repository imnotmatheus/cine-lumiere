using ReservaEspectaculos_D.Models;
using System;

namespace ReservaEspectaculos_D.Utils
{
    public static class ReservaHelper
    {
        public static bool VerificarDisponibilidad(Funcion funcion, int cantidadAReservar)
        {
            return funcion.ButacasDisponibles >= cantidadAReservar;
        }

        public static void ActualizarEstadoReserva(Reserva reserva)
        {
            if (reserva != null)
            {
                var (horaActual, fechaActual) = DateTimeHelper.ObtenerInfoDateTime();

                if (reserva.Funcion.Fecha < fechaActual || (reserva.Funcion.Fecha == fechaActual && reserva.Funcion.Hora < horaActual))
                {
                    reserva.EstadoReserva = EstadoReserva.Inactiva;
                }
            }
        }
    }
}

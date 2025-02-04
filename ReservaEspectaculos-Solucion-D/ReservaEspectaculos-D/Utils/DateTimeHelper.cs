using System;
using ReservaEspectaculos_D.Models;

namespace ReservaEspectaculos_D.Utils
{
    public class DateTimeHelper
    {
        public static (TimeOnly horaActual, DateOnly fechaActual) ObtenerInfoDateTime()
        {
            DateTime ahora = DateTime.Now;
            var horaActual = TimeOnly.FromDateTime(ahora);
            var fechaActual = DateOnly.FromDateTime(ahora);
            return (horaActual, fechaActual);
        }
    }
}

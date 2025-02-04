using System;

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

        public static DateOnly ObtenerFechaActual()
        {
            return DateOnly.FromDateTime(DateTime.Now);
        }
    }
}

using ReservaEspectaculos_D.Data;
using ReservaEspectaculos_D.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReservaEspectaculos_D.Utils
{
    public class PeliculaHelper
    {
        const int RangoDiasFuturos = 7;
        public static IEnumerable<Pelicula> ObtenerPeliculasEnExibicion(ReservaEspectaculosDb _context)
        {
            var (horaActual, fechaActual) = DateTimeHelper.ObtenerInfoDateTime();
            var fechaLimite = fechaActual.AddDays(RangoDiasFuturos);

            return _context.Peliculas.
                Where(p => p.Funciones.Any(f =>
                f.Fecha <= fechaLimite &&
                    (f.Fecha > fechaActual || (f.Fecha == fechaActual && f.Hora > horaActual)) &&
                    f.Confirmada == true));
        }

        public static IEnumerable<Funcion> ObtenerFuncionesFuturasDePelicula(Pelicula pelicula)
        {
            var (horaActual, fechaActual) = DateTimeHelper.ObtenerInfoDateTime();
            var fechaLimite = fechaActual.AddDays(RangoDiasFuturos);

            return pelicula.Funciones.Where(f =>
                    (f.Fecha > fechaActual || (f.Fecha == fechaActual && f.Hora >= horaActual)) &&
                    f.Fecha <= fechaLimite &&
                    f.Confirmada == true);
        }
    }
}

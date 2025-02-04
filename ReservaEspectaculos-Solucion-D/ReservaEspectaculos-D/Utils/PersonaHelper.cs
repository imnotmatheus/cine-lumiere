using System;
using Microsoft.EntityFrameworkCore;
using ReservaEspectaculos_D.Data;

namespace ReservaEspectaculos_D.Utils
{
    public class PersonaHelper
    {
        public string GenerarLegajo()
        {
            string legajo = null;
            int numero = GetRandomNumber(DateTime.Now);
            legajo = $"{numero}-{GetVerificador(numero)}";
            return legajo;
        }
        private int GetVerificador(int numero)
        {
            numero = Math.Abs(numero);
            int hash = ((numero * 37) + 17) % 90 + 10;
            return hash;
        }
        private int GetRandomNumber(DateTime fecha)
        {
            return (fecha.Year % 100) * 1000000 +
                   fecha.Month * 10000 +
                   fecha.Day * 1000 +
                   fecha.Hour * 100 +
                   fecha.Minute * 10 +
                   fecha.Second * 1 + fecha.Microsecond;
        }
    }
}

using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReservaEspectaculos_D.Data;

namespace ReservaEspectaculos_D.Utils
{
    public class SalaHelper
    {
        public static bool SalaExists(int id, ReservaEspectaculosDb _context)
        {
            return _context.Salas.Any(e => e.Id == id);
        }
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ReservaEspectaculos_D.Utils
{
    public class ExceptionHandler
    {
        public void ProcesarIndicesUnicos(DbUpdateException ex, ModelStateDictionary modelState, string indice, string mensajeError)
        {
            SqlException innerException = ex.InnerException as SqlException;
            if (innerException != null && (innerException.Number == 2627 ||innerException.Number == 2601))
            {
                modelState.AddModelError(indice, mensajeError);
            }
            else
            {
                modelState.AddModelError(string.Empty, ex.Message);
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using ReservaEspectaculos_D.Utils;
using System.ComponentModel.DataAnnotations;

namespace ReservaEspectaculos_D.Models
{
    
    public class Empleado : Persona
    {
        [Required(ErrorMessage = ErrorHelper.Requerido)]
        public string Legajo { get; set; }

    }
}

using Microsoft.AspNetCore.Mvc;
using ReservaEspectaculos_D.Utils;
using System.ComponentModel.DataAnnotations;

namespace ReservaEspectaculos_D.Models.ViewModels
{
    public class RegistrarEmpleado
    {
        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [EmailAddress(ErrorMessage = ErrorHelper.Email)]
        [Remote(action: "EmailDisponible", controller: "Account")]
        public string Email { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [MaxLength(35, ErrorMessage = ErrorHelper.StrMax)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [MaxLength(35, ErrorMessage = ErrorHelper.StrMax)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [RegularExpression(RegExHelper.DNI,
            ErrorMessage = ErrorHelper.DNI)]
        public string DNI { get; set; }

        [RegularExpression(RegExHelper.Telefono, ErrorMessage = ErrorHelper.TelefonoFormato)]
        [Length(10, 10, ErrorMessage = ErrorHelper.TelefonoFormato)]
        public string Telefono { get; set; }

        [MaxLength(50, ErrorMessage = ErrorHelper.StrMax)]
        public string Direccion { get; set; }

        public string Password { get; set; } = Config.PasswordPorDefecto;
    }
}

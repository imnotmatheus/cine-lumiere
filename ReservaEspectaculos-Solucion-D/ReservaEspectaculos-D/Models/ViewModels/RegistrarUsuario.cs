using Microsoft.AspNetCore.Mvc;
using ReservaEspectaculos_D.Utils;
using System.ComponentModel.DataAnnotations;

namespace ReservaEspectaculos_D.Models.ViewModels
{
    public class RegistrarUsuario
    {

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [EmailAddress(ErrorMessage = ErrorHelper.Email)]
        [Remote(action: "EmailDisponible", controller: "Account")]
        public string Email { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = ErrorHelper.StrMaxMin)]
        [RegularExpression(RegExHelper.Password,
        ErrorMessage = ErrorHelper.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = ErrorHelper.ConfirmacionPassword)]
        public string ConfirmacionPassword { get; set; }

        [Display(Name = "Recordar contraseña")]
        public bool RememberMe { get; set; } = false;

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [MaxLength(35, ErrorMessage = ErrorHelper.StrMax)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [MaxLength(35, ErrorMessage = ErrorHelper.StrMax)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [RegularExpression(RegExHelper.DNI, ErrorMessage = ErrorHelper.DNI)]
        public string DNI { get; set; }

        [RegularExpression(RegExHelper.Telefono, ErrorMessage = ErrorHelper.TelefonoFormato)]
        [Length(10, 10, ErrorMessage = ErrorHelper.TelefonoFormato)]
        public string Telefono { get; set; }

        [MaxLength(50, ErrorMessage = ErrorHelper.StrMax)]
        public string Direccion { get; set; }

    }
}

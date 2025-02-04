using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReservaEspectaculos_D.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace ReservaEspectaculos_D.Models
{
    public class Persona : IdentityUser<int> {

        // public int Id { get; set; }

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [MaxLength(35, ErrorMessage = ErrorHelper.StrMax)]
        public string Nombre { get; set; }

        [Required(ErrorMessage =ErrorHelper.Requerido)]
        [MaxLength (35, ErrorMessage = ErrorHelper.StrMax)]
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

        [Required(ErrorMessage = ErrorHelper.Requerido)]
        [EmailAddress(ErrorMessage = ErrorHelper.EmailFormato)]
        public override string Email {
            get { return base.Email; } 
            set { base.Email = value; }
        }

        [Display(Name = "Nombre de usuario")]
        public override string UserName { 
            get { return base.UserName; }
            set { base.UserName = value; }
        }

        //[Required(ErrorMessage = ErrorHelper.Requerido)]
        //[DataType(DataType.Password)]
        //[StringLength(100, MinimumLength = 8, ErrorMessage = ErrorHelper.StrMaxMin)]
        //[RegularExpression(RegExHelper.Password,
        //ErrorMessage = ErrorHelper.Password)]
        //[Display(Name = "Contraseña")]
        //public string Password { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = ErrorHelper.Data)]
        [Display(Name = "Fecha de alta")]
        public DateTime FechaAlta { get; set; } = DateTime.Now;
    }
}

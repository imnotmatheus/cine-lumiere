using ReservaEspectaculos_D.Utils;
using System.ComponentModel.DataAnnotations;

namespace ReservaEspectaculos_D.Models.ViewModels
{
    public class CambioPerfil
    {
        public int PersonaId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }

        [RegularExpression(RegExHelper.Telefono, ErrorMessage = ErrorHelper.TelefonoFormato)]
        [Length(10, 10, ErrorMessage = ErrorHelper.TelefonoFormato)]
        public string Telefono { get; set; }

        [MaxLength(50, ErrorMessage = ErrorHelper.StrMax)]
        public string Direccion { get; set; }
        public string Email { get; set; }
    }
}

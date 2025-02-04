using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ReservaEspectaculos_D.Models.ViewModels
{
    public class Login
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Recordame")]
        public bool RememberMe { get; set; }
    }
}

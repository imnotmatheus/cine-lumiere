using System;

namespace ReservaEspectaculos_D.Utils
{
    public static class ErrorHelper
    {
        public const string Requerido = "El campo {0} es requerido";
        public const string StrMax = "El campo {0} no puede tener más de {1} caracteres";
        public const string StrMin = "El campo {0} debe tener al menos {1} caracteres";
        public const string StrMaxMin = "El campo {0} debe tener entre {2} y {1} caracteres";
        public const string NumRange = "El campo {0} debe estar entre {1} y {2}";
        public const string Data = "El campo {0} debe ser de tipo {1}";
        public const string EmailFormato = "El campo {0} debe tener formato email";
        public const string Password = "La {0} debe tener una mayúscula, una minúscula, un número y un caracter especial (@$!%*?&).";
        public const string ConfirmacionPassword = "Las contraseñas deben coincidir.";
        public const string DNI = "{0} debe tener 8 dígitos. Matrículas menores a 10.000.000 deben ser antecedidas por la sigla 'F' o 'M', de acuerdo al sexo de la misma";
        public const string TelefonoFormato = "Ingresar {0} con codigo de área y número (10 dígitos)";
        public const string Email = "Este email ya está en uso.";
        public const string Numero = "Este número ya está en uso";
        public const string Nombre = "Este nombre ya está en uso.";
        public const string Titulo = "Este titulo ya está en uso.";
        public const string Legajo = "Este legajo ya está en uso.";
        public const string FuncionDuplicada = "Ya existe una función en esta sala con el mismo horario, fecha y película.";
        public const string SalaNumero = "Ya existe una sala con este numero";
        public static string ErrorGenerico(Exception e)
        {
            return $"Error no esperado: {e.InnerException?.Message ?? "Sin detalles adicionales"}";
        }
    }
}

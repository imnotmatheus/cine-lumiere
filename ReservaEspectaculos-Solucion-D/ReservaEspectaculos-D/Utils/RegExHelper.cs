namespace ReservaEspectaculos_D.Utils
{
    public class RegExHelper
    {
        public const string DNI = @"^[MmFf\d]\d{7}$";
        public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
        public const string Telefono = @"^(11|[23]\d{2,4})\d{6,8}$";
    }
}

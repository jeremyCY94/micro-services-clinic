using Microsoft.AspNetCore.Identity;

namespace authServices.Core.Entities
{
    public class Usuario : IdentityUser
    {
        public string Nombre { get; set; }  
        public string Apellido { get; set; }
        public string direccion { get; set; }

    }
}

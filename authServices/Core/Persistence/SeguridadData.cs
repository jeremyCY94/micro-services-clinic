using authServices.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System.Net.WebSockets;

namespace authServices.Core.Persistence
{
    public class SeguridadData
    {
        public static async Task InsertarUsuario(SeguridadContexto context, UserManager<Usuario> usuarioManager)
        {
            if (!usuarioManager.Users.Any())
            {
                Usuario usuario = new Usuario
                {
                    Nombre = "Jeremy",
                    Apellido = "Calderon",
                    direccion = "av huanuco",
                    UserName = "dabcy",
                    Email= "jeremycy94@gmail.com",
                };

                await usuarioManager.CreateAsync(usuario, "Aa123456_1");
            }
        }
    }
}

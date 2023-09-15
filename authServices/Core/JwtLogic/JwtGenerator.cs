using authServices.Core.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace authServices.Core.JwtLogic
{
    public class JwtGenerator : IJwtGenerator
    {
        private IConfiguration _configuration;
        public JwtGenerator(IConfiguration configuration) {
            _configuration= configuration;
        }
        public string CreateToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim("id", usuario.Id),
                new Claim("nombre", usuario.Nombre),
                new Claim("apellido", usuario.Apellido),
            };
            var valuekey = _configuration.GetSection("token").GetSection("key").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(valuekey));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject= new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(20),
                SigningCredentials= credential
            };

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription);
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }

        private bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate, TokenValidationParameters @param)
        {
            if (expires != null)
            {
                return expires > DateTime.UtcNow;
            }
            return false;
        }
    }
}

namespace authServices.Core.JwtLogic
{
    public class UsuarioSession : IUsuarioSession
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioSession(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUsuarioSession()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var username = user.Claims?.FirstOrDefault(x => x.Type == "id")?.Value;

            return username;
        }
    }
}

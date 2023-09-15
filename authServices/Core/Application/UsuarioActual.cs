using authServices.Core.Dto;
using authServices.Core.Entities;
using authServices.Core.JwtLogic;
using authServices.Core.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace authServices.Core.Application
{
    public class UsuarioActual
    {
    
        public class UsuarioActualCommand : IRequest<UsuarioDto>
        {

        }

        public class UsuarioActualHandler : IRequestHandler<UsuarioActualCommand, UsuarioDto>
        {
            private readonly IUsuarioSession _usuarioSession;
            private readonly UserManager<Usuario> _userManager;
            private readonly IMapper _mapper;
            private readonly IJwtGenerator _ijwtGenerator;
            public UsuarioActualHandler(IUsuarioSession usuarioSession, UserManager<Usuario> userManager, IMapper mapper, IJwtGenerator ijwtGenerator)
            {
                _usuarioSession = usuarioSession;
                _userManager = userManager;
                _mapper = mapper;
                _ijwtGenerator = ijwtGenerator;
            }
            public async Task<UsuarioDto> Handle(UsuarioActualCommand request, CancellationToken cancellationToken)
            {
                var usuario = await _userManager.FindByIdAsync(_usuarioSession.GetUsuarioSession());

                if (usuario != null) { 
                    var dto = _mapper.Map<Usuario, UsuarioDto>(usuario);
                    return dto;
                }
                else
                {
                    throw new UnauthorizedAccessException("no se encontro el usuario");
                }

            }
        }

    }
}

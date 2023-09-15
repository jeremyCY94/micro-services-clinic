using authServices.Core.Dto;
using authServices.Core.Entities;
using authServices.Core.JwtLogic;
using authServices.Core.Persistence;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace authServices.Core.Application
{
    public class Login
    {

        public class UsuarioLoginCommand : IRequest<UsuarioDto>
        {
            public string Email { get; set; }
            public string Password { get; set; }

        }

        public class UsuarioLoginValidation : AbstractValidator<UsuarioLoginCommand>
        {
            public UsuarioLoginValidation()
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class UsuarioLoginHandler : IRequestHandler<UsuarioLoginCommand, UsuarioDto>
        {
            private readonly SeguridadContexto _context;
            private readonly UserManager<Usuario> _userManager;
            private readonly IMapper _mapper;
            private readonly IJwtGenerator _ijwtGenerator;
            private readonly SignInManager<Usuario> _signInManager;
            public UsuarioLoginHandler(SeguridadContexto context, UserManager<Usuario> userManager, IMapper mapper, IJwtGenerator ijwtGenerator, SignInManager<Usuario> signInManager)
            {
                _context = context;
                _userManager = userManager;
                _mapper = mapper;
                _ijwtGenerator = ijwtGenerator;
                _signInManager = signInManager;

            }
            public async Task<UsuarioDto> Handle(UsuarioLoginCommand request, CancellationToken cancellationToken)
            {
                var usuario = await _userManager.FindByEmailAsync(request.Email);

                if (usuario != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(usuario, request.Password, false);
                    if (result.Succeeded)
                    {
                        var dto = _mapper.Map<Usuario, UsuarioDto>(usuario);
                        dto.token = _ijwtGenerator.CreateToken(usuario);
                        return dto;
                    }
                    else 
                    {
                        throw new Exception("Usuario o contraseña incorrecto");
                    }
                }
                else
                {
                    throw new Exception("El usuario no existe");
                }
            }
        }

    }
}

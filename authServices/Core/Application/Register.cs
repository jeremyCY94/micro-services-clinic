using authServices.Core.Dto;
using authServices.Core.Entities;
using authServices.Core.JwtLogic;
using authServices.Core.Persistence;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace authServices.Core.Application
{
    public class Register
    {
        public class UsuarioRegisterCommand : IRequest<UsuarioDto>
        {
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class UsuarioRegisterValidation : AbstractValidator<UsuarioRegisterCommand>
        {
            public UsuarioRegisterValidation() {
                RuleFor(x => x.Nombre).NotEmpty();
                RuleFor(x => x.Apellido).NotEmpty();
                RuleFor(x => x.UserName).NotEmpty();
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class UsuarioRegisterHandler : IRequestHandler<UsuarioRegisterCommand, UsuarioDto>
        {
            private readonly SeguridadContexto _context;
            private readonly UserManager<Usuario> _userManager;
            private readonly IMapper _mapper;
            private readonly IJwtGenerator _ijwtGenerator;
            public UsuarioRegisterHandler(SeguridadContexto context, UserManager<Usuario> userManager, IMapper mapper, IJwtGenerator ijwtGenerator)
            {
                _context= context;
                _userManager= userManager;
                _mapper= mapper; 
                _ijwtGenerator= ijwtGenerator;
            }
            public async  Task<UsuarioDto> Handle(UsuarioRegisterCommand request, CancellationToken cancellationToken)
            {
                var existe = await _context.Users.Where(x => x.Email == request.Email).AnyAsync();
                if (!existe)
                {
                    existe = await _context.Users.Where(x => x.UserName == request.UserName).AnyAsync();
                    if (!existe)
                    {
                        var usuario = new Usuario
                        {
                            Nombre = request.Nombre,
                            Apellido = request.Apellido,
                            Email = request.Email,
                            UserName = request.UserName,
                            direccion = ""
                        };

                        var resultado = await _userManager.CreateAsync(usuario, request.Password);

                        if (resultado.Succeeded)
                        {
                            var dto = _mapper.Map<Usuario, UsuarioDto>(usuario);
                            dto.token = _ijwtGenerator.CreateToken(usuario);
                            return dto;
                        }
                        else
                        {
                            throw new Exception("No se Pudo Registrar el Usuario");
                        }
                    }
                    else
                    {
                        throw new Exception("El UserName del Usuario ya Existe en la Base de datos");
                    }
                }
                else
                {
                    throw new Exception("El Email del Usuario ya Existe en la Base de datos");
                }
 

            }
        }


    }
}

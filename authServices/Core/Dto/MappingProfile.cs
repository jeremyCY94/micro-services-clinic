using authServices.Core.Entities;
using AutoMapper;

namespace authServices.Core.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<Usuario, UsuarioDto>();
        }
    }
}

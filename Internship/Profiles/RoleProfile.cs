using AutoMapper;
using Internship.Models;

namespace Internship.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile() 
        {
            CreateMap<Role, RoleDTO>()
                .ForMember(
                    dest => dest.RoleName,
                    input => input.MapFrom(src => src.Name))
                .ReverseMap();
            CreateMap<Role, NewRoleDTO>();
            CreateMap<Role, RoleForJsonDTO>();
        }
    }
}

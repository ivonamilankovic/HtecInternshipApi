using AutoMapper;
using Internship.Models;

namespace Internship.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile() 
        {
            CreateMap<UserDataDTO, User>()
                .ForPath(
                    dest => dest.Role.Name, 
                    input => input.MapFrom(src => src.RoleName))
                .ReverseMap();
            CreateMap<UserCreateUpdateDTO, User>()
                .ForPath(
                    dest => dest.Role.Id, 
                    input => input.MapFrom(src => src.RoleId))
                .ReverseMap();
            CreateMap<User, UserNameEmailDTO>();
            CreateMap<User, NewUserDTO>();
            CreateMap<User, UserForJsonDTO>();
        }
    }
}

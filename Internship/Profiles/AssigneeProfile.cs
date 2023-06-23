using AutoMapper;
using Internship.Models;

namespace Internship.Profiles
{
    public class AssigneeProfile : Profile
    {
        public AssigneeProfile() 
        {
            CreateMap<User, MentorAssigneeInfoDTO>()
                .ForPath(
                    dest => dest.mentor.MentorName,
                    input => input.MapFrom(src=> src.Username))
                .ForPath(
                    dest => dest.mentor.MentorId,
                    input => input.MapFrom(src=> src.Id))
                .ForPath(
                    dest => dest.assignee.AssigneeName,
                    input => input.MapFrom(src=> src.Assignee.Username))
                .ForPath(
                    dest => dest.assignee.AssigneeId,
                    input => input.MapFrom(src=> src.Assignee.Id))
                ;
            CreateMap<User, AssigneeForJsonDTO>();
        }
    }
}

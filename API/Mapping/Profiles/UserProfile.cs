using API.DTO.Requests.User;
using Application.UseCases.Commands.Requests.User;

using AutoMapper;

namespace API.Mapping.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDto, CreateUserRequest>();
        }
    }
}

using API.DTO.Requests.User;
using Application.UseCases.Commands.Requests.User;

using AutoMapper;

namespace API.Mapping.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // API request -> Application DTO
            CreateMap<CreateUserDto, CreateUserRequest>();
        }
    }
}

using API.DTO.Requests.User;
using Application.UseCases.DTO.User;
using AutoMapper;

namespace API.Mapping.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // API request -> Application DTO
            CreateMap<CreateUserRequest, CreateUserDto>();
        }
    }
}

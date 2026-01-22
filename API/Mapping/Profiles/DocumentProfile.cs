using API.DTO.Requests.Document;
using Application.UseCases.Commands.Requests.Document;
using AutoMapper;

namespace API.Mapping.Profiles
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<CreateDocumentDto, CreateDocumentRequest>();
        }
    }
}

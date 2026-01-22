using API.DTO.Requests.DocumentType;
using Application.UseCases.Commands.Requests.DocumentType;
using AutoMapper;

namespace API.Mapping.Profiles
{
    public class DocumentTypeProfile : Profile
    {
        public DocumentTypeProfile() 
        {
            CreateMap<CreateDocumentTypeDto, CreateDocumentTypeRequest>()
                .ForMember(d => d.Fields, opt => opt.MapFrom(s => s.Fields));

            CreateMap<CreateDocumentTypeFieldDto, CreateDocumentTypeFieldRequest>();
        }
    }
}

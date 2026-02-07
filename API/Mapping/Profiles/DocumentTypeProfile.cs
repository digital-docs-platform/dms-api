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
                .ForMember(d => d.FieldDefinitions, opt => opt.MapFrom(s => s.Fields));

            CreateMap<CreateDocumentTypeFieldDto, CreateDocumentTypeFieldRequest>();
        }
    }
}

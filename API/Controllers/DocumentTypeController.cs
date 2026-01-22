using API.DTO.Requests.DocumentType;
using API.DTO.Response;
using Application.UseCaseHandling;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.DocumentType;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/document-type")]
    [ApiController]
    public class DocumentTypeController : ControllerBase
    {
        private readonly ICommandHandler _commandHandler;
        private readonly IMapper _mapper;
        public DocumentTypeController(ICommandHandler commandHandler, IMapper mapper) 
        {
            _commandHandler = commandHandler;
            _mapper = mapper;
        }


        // POST api/<DocumentTypeController>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateDocumentTypeDto reqDto, [FromServices] ICreateDocumentTypeCommand command , CancellationToken ct)
        {

            CreateDocumentTypeRequest request = _mapper.Map<CreateDocumentTypeRequest>(reqDto);
            await _commandHandler.HandleAsync(command, request, ct);

            return Ok( new SuccessResponse
            {
                Message = "You have successfully create new Document Type . . .",
                Data = null
            });
        }

    }
}

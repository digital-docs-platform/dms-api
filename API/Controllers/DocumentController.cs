using API.DTO.Requests.Document;
using API.DTO.Response;
using Application.UseCaseHandling;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.Document;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICommandHandler _commandHandler;
        public DocumentController(IMapper mapper, ICommandHandler commandHandler)
        {
            _mapper = mapper;
            _commandHandler = commandHandler;
        }
        // POST api/<DocumentController>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDocumentDto dto, [FromServices] ICreateDocumentCommand command,CancellationToken ct)
        {

            CreateDocumentRequest request = _mapper.Map<CreateDocumentRequest>(dto);
            await _commandHandler.HandleAsync(command , request, ct);

            


            return StatusCode(201, new SuccessResponse
            {
                Message = "Successfully added new Document!",
                Data = null
            });
        }

    }
}

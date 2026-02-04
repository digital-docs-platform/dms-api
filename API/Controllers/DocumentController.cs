using API.DTO.Requests.Document;
using API.DTO.Response;
using Application.UseCaseHandling;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.Document;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Search;
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
        private readonly IQueryHandler _queryHandler;
        public DocumentController(IMapper mapper, ICommandHandler commandHandler, IQueryHandler queryHandler)
        {
            _mapper = mapper;
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
        }
        // POST api/<DocumentController>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDocumentRequest request, [FromServices] ICreateDocumentCommand command,CancellationToken ct)
        {
            await _commandHandler.HandleAsync(command , request, ct);

            


            return StatusCode(201, new SuccessResponse
            {
                Message = "Successfully added new Document!",
                Data = null
            });
        }

    }
}

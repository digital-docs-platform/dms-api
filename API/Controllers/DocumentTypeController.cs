using API.DTO.Requests.DocumentType;
using API.DTO.Response;
using Application.UseCaseHandling;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.DocumentType;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Search;
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
        private readonly IQueryHandler _queryHandler;
        private readonly IMapper _mapper;
        public DocumentTypeController(ICommandHandler commandHandler, IQueryHandler queryHandler,IMapper mapper) 
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
            _mapper = mapper;
        }

        [HttpGet("{DocumentTypeId:guid}")]
        public async Task<IActionResult> GetDoucmentTypeById(
            [FromRoute] DocumentTypeIdSearch search,
            [FromServices] IGetDocumentTypeByIdQuery query,
            CancellationToken ct) 
        
        {
            var response = await _queryHandler.HandleAsync(query, search, ct);

            return Ok( new SuccessResponse
            {
                Data = response,
                Message = "Successfully found document type information!"
            });
        }


        // POST api/<DocumentTypeController>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateDocumentTypeRequest request, [FromServices] ICreateDocumentTypeCommand command , CancellationToken ct)
        {

            await _commandHandler.HandleAsync(command, request, ct);

            return Ok(new SuccessResponse
            {
                Message = "You have successfully create new Document Type . . .",
                Data = new
                {
                    request.DocumentTypeId
                }
            }); 
        }

        [HttpPost("{documentTypeId:guid}/documents/search")]
        public async Task<IActionResult> GetDocumentsByDocumentType(
            Guid documentTypeId,
            [FromBody] GetDocumentsByDocumentTypeIdSearch search,
            [FromServices] IGetDocumentsByDocumentTypeIdQuery query,
            CancellationToken ct)
        {

            search.DocumentTypeId = documentTypeId;
            var result = await _queryHandler.HandleAsync(query, search, ct);


            return Ok(new SuccessResponse
            {
                Data = result,
                Message = "Successfully fetched documents for document type . . ."
            });
        }

    }
}

using API.DTO.Response;
using Application.UseCaseHandling;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.Document;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Search;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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




        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(
            [FromForm] Guid documentTypeId,
            [FromForm] string? title,
            [FromForm] string? fieldsInputJson,
            [FromForm] List<IFormFile>? files,
            [FromServices] ICreateDocumentCommand command,
            CancellationToken ct)
        {
            var fieldsInput = string.IsNullOrEmpty(fieldsInputJson)
                ? new List<CreateDocumentFieldInputRequest>()
                : JsonSerializer.Deserialize<List<CreateDocumentFieldInputRequest>>(fieldsInputJson) ?? new();

            var request = new CreateDocumentRequest
            {
                DocumentTypeId = documentTypeId,
                Title = title,
                FieldsInput = fieldsInput,
                Files = (files ?? new()).Select((f, i) => new DocumentFileInput
                {
                    Content = f.OpenReadStream(),
                    FileName = f.FileName,
                    ContentType = f.ContentType,
                    SizeInBytes = f.Length,
                    Order = i
                }).ToList()
            };

            await _commandHandler.HandleAsync(command, request, ct);

            return StatusCode(201, new SuccessResponse
            {
                Message = "Successfully added new Document!",
                Data = new { DocumentId = request.Id }
            });
        }






        [HttpGet("{documentId:guid}")]
        public async Task<IActionResult> GetDocumentById(
            [FromRoute] DocumentIdSearch search,
            [FromServices] IGetDocumentByIdQuery query,
            CancellationToken ct)
        {

            var result = await _queryHandler.HandleAsync(query, search, ct);

            return Ok( new SuccessResponse
            {
                Data = result,
                Message = "Successfully found Document."
            });
        }

    }
}

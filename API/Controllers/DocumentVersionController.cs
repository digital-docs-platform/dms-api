using API.DTO.Response;
using Application.UseCaseHandling;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Search;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/document-versions")]
    [ApiController]
    public class DocumentVersionController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        public DocumentVersionController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }
        // GET: api/<DocumentVersionController>
        [HttpGet("{DocumentId:guid}")]
        public async Task<IActionResult> GetDocumentVersionsByDocumentId(
            [FromRoute] DocumentIdSearch search,
            [FromServices] IGetDocumentVersionsByDocumentIdQuery query,
            CancellationToken ct)
        {

            var result = await _queryHandler.HandleAsync(query, search, ct);

            return Ok( new SuccessResponse
            {
                Data = result,
                Message = "Successfully found document versions."
            });
        }

       
    }
}

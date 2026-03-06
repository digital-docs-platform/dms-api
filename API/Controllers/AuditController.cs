using API.DTO.Response;
using Application.UseCaseHandling;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Search;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        public AuditController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }


        [HttpPost]
        public async Task<IActionResult> GetUserAuditHistory(
            [FromServices] IGetUserAuditHistoryQuery query,
            [FromBody] GetUserAuditHistorySearch search,
            CancellationToken ct)
        {

            var result = await _queryHandler.HandleAsync(query, search, ct);

            return Ok(new SuccessResponse
            {
                Data = result,
                Message = "Successfully get User Audit History. . ."
            });
        }

        [HttpPost("document")]
        public async Task<IActionResult> GetDocumentAuditHistory(
            [FromServices] IGetDocumentAuditHistoryQuery query,
            [FromBody] GetDocumentAuditHistorySearch search,
            CancellationToken ct)
        {
            var result = await _queryHandler.HandleAsync(query, search, ct);
            return Ok(new SuccessResponse
            {
                Data = result,
                Message = "Successfully get Document Audit History. . ."
            });


        }
    }
}

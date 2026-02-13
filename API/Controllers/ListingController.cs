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
    public class ListingController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        public ListingController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }


        // GET: api/<ListingController>
        [HttpPost("documents/export")]
        public async Task<IActionResult> ExportDocumentsListing(
            [FromBody] ExportDocumentsListingSearch search,
            [FromServices] IExportDocumentsListingQuery query,
            CancellationToken ct
            )
        {
            var file = await _queryHandler.HandleAsync(query, search, ct);


            return File(file.Content, file.ContentType, file.FileName);
        }

    }
}

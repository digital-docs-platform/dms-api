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
    public class PermissionController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        public PermissionController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }


        // GET: api/<PermissionController>
        [HttpGet("catalog/{Id:guid}")]
        public async Task<IActionResult> GetPermissionCatalog(
                                        [FromServices] IGetPermissionsCatalogQuery query,
                                        [FromRoute] IdSearch search,
                                        CancellationToken ct)
        {
            var result = await _queryHandler.HandleAsync(query, search, ct);

            return Ok( new SuccessResponse
            {
                Data = result,
                Message = "Successfully found Permissions Catalog"
            });
        }

     
    }
}

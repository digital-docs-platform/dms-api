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
    public class GroupController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        public GroupController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        [HttpGet("{id:guid}/permissions")]
        public async Task<IActionResult> GetGroupPermissions(
                                         [FromServices] IGetGroupPermissionsQuery query,
                                         [FromRoute] IdSearch<Guid> search,
                                         CancellationToken ct)
        {
            var result = await _queryHandler.HandleAsync(query, search, ct);  

            return Ok(new SuccessResponse
            {
                Data = result,
                Message = "Successfully get group permissions"
            });
        }

        
    }
}

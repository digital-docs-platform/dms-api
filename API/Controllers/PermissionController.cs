using API.DTO.Response;
using Application.UseCaseHandling;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.Permissions;
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
        private readonly ICommandHandler _commandHandler;
        public PermissionController(IQueryHandler queryHandler, ICommandHandler commandHandler)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
        }


        // GET: api/<PermissionController>
        //Id == UserId
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


        [HttpPut("update/user")]
        public async Task<IActionResult> UpdateUserPermissions(
                                         [FromBody] UpdateUserPermissionsRequest request,
                                         [FromServices] IUpdateUserPermissionsCommand command,
                                         IGetPermissionsCatalogQuery query,
                                         CancellationToken ct)
        {
            await _commandHandler.HandleAsync(command, request, ct);

            var permissions = await _queryHandler.HandleAsync(query, new IdSearch { Id = request.UserId }, ct);

            return Ok(new SuccessResponse
            {
                Message = "Successfully updated user permissions!",
                Data = permissions
            });
        }
        
     
    }
}

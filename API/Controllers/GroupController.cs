using API.DTO.Response;
using Application.UseCaseHandling;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.Group;
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
        private readonly ICommandHandler _commandHandler;
        public GroupController(IQueryHandler queryHandler, ICommandHandler commandHandler)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
        }



        [HttpGet("{Id:guid}")]
        public async Task<IActionResult> GetGroupById(
                                        [FromServices] IGetGroupByIdQuery query,
                                        [FromRoute] IdSearch<Guid> search,
                                        CancellationToken ct)
        {
            var result = await _queryHandler.HandleAsync(query, search, ct);  
            return Ok(new SuccessResponse
            {
                Data = result,
                Message = "Successfully found group"
            });
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup(
                                        [FromServices] ICreateGroupCommand command,
                                        [FromBody] CreateGroupRequest request,
                                        CancellationToken ct)
        {
            await _commandHandler.HandleAsync(command, request, ct);

            return Ok(new SuccessResponse
            {
                Data = null,
                Message = "Successfully created new Group"
            });
        }

        
    }
}

using API.DTO.Requests.User;
using API.DTO.Response;
using Application.UseCaseHandling;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.User;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Search;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ICommandHandler _commandHandler;
        private readonly IQueryHandler _queryHandler;
        private readonly IMapper _mapper;

        public UserController(ICommandHandler commandHandler, IQueryHandler queryHandler,IMapper mapper)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
            _mapper = mapper;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById([FromRoute] IdSearch search, [FromServices] IGetUserByIdQuery query, CancellationToken ct)
        {
            var response = await _queryHandler.HandleAsync(query, search, ct);


            return StatusCode(201, new SuccessResponse
            {
                Message = "Successfully found user",
                Data = new
                {
                    User = response
                }
            });
        }

        // POST api/<UserController>
        [HttpPost("new")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, [FromServices] ICreateUserCommand command, CancellationToken ct)
        {
           

            await _commandHandler.HandleAsync(command, request, ct);

            return Ok(new SuccessResponse
            {
                Message = "You have successfully created new user!",
                Data = new 
                {
                    UserId = request.Id
                }
            });
        }

        [HttpPut("{id:guid}/edit")]
        public async Task<IActionResult> UpdateUserProfile(
            [FromServices]IUpdateUserProfileCommand command,
            [FromBody] UpdateUserProfileRequest request,
            [FromRoute] Guid id,
            CancellationToken ct)
        {

            request.UserId = id;
            await _commandHandler.HandleAsync(command, request, ct);

            return Ok(new SuccessResponse
            {
                Data = null,
                Message = "Successfully updated user informations!"
            });
        }

      
    }
}

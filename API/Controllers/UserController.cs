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

        [HttpGet("{id}")]
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
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto, [FromServices] ICreateUserCommand command, CancellationToken ct)
        {
            CreateUserRequest request = _mapper.Map<CreateUserRequest>(dto);

            await _commandHandler.HandleAsync(command, request, ct);

            return Ok(new SuccessResponse
            {
                Message = "You have successfully created new user!",
                Data = null
            });
        }

      
    }
}

using API.DTO.Requests.User;
using API.DTO.Response;
using Application.UseCaseHandling;
using Application.UseCases.Commands;
using Application.UseCases.DTO.User;
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
        private readonly IMapper _mapper;

        public UserController(ICommandHandler commandHandler, IMapper mapper)
        {
            _commandHandler = commandHandler;
            _mapper = mapper;
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest req, [FromServices] ICreateUserCommand command, CancellationToken ct)
        {
            var dto = _mapper.Map<CreateUserDto>(req);

            await _commandHandler.HandleAsync(command, dto, ct);

            return Ok(new SuccessResponse
            {
                Message = "You have successfully created new user!",
                Data = null
            });
        }

      
    }
}

using API.DTO.Requests.Auth;
using API.DTO.Response;
using Application.jwt;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtManager _jwtManager;
        public AuthController(IJwtManager manager)
        {
            _jwtManager = manager;
        }

        // POST api/<AuthController>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest req, CancellationToken ct)
        {
            string token = await _jwtManager.MakeToken(req.Email, req.Password, ct);
            return Ok(new SuccessResponse
            {
                Message = "You have successfully loged in . . .",
                Data = new { Token = token }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CancellationToken ct)
        {


            return StatusCode(201, new SuccessResponse
            {
                Message = "Account created.",
                Data = null
            });
        }

    }
}

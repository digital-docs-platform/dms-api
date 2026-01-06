using API.Auth.Cookies;
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
        private readonly IAuthCookieService _cookieService;
     
        public AuthController(IJwtManager manager, IAuthCookieService cookieService)
        {
            _jwtManager = manager;
            _cookieService = cookieService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest req, CancellationToken ct)
        {

            string token = await _jwtManager.MakeToken(req.Email, req.Password, ct);
            _cookieService.SetAccessToken(Response, token);
            
            return Ok(new SuccessResponse
            {
                Message = "Logged in",
                Data = null
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

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _cookieService.ClearAccessToken(Response);
            return Ok();
        }

    }
}

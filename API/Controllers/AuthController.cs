using API.Auth.Cookies;
using API.DTO.Requests.Auth;
using API.DTO.Response;
using Application.jwt;
using Application.UseCaseHandling;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Response;
using Application.UseCases.Queries.Search;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtManager _jwtManager;
        private readonly IAuthCookieService _cookieService;
        private readonly IQueryHandler _queryHandler;
     
        public AuthController(IJwtManager manager, IAuthCookieService cookieService, IQueryHandler queryHandler)
        {
            _jwtManager = manager;
            _cookieService = cookieService;
            _queryHandler = queryHandler;
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me([FromServices] IGetMeQuery query, CancellationToken ct )
        {
            var result = await _queryHandler.HandleAsync( query, new EmptySearch(),ct);


            return Ok(new SuccessResponse
            {
                Message = "Successfully found user info . . .",
                Data = result
            });
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

        [HttpDelete("logout")]
        public IActionResult Logout()
        {
            _cookieService.ClearAccessToken(Response);

            return StatusCode(204,  new SuccessResponse
            {
                Data = null,
                Message = "User successfully logged out"
            });
        }

    }
}

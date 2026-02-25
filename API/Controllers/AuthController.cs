using API.Auth.Cookies;
using API.DTO.Requests.Auth;
using API.DTO.Response;
using Application;
using Application.jwt;
using Application.Logging;
using Application.UseCaseHandling;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Response;
using Application.UseCases.Queries.Search;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IAuditLogger _auditLogger;
        private readonly IApplicationActor _actor;

        public AuthController(
            IJwtManager manager,
            IAuthCookieService cookieService,
            IQueryHandler queryHandler,
            IAuditLogger auditLogger,
            IApplicationActor actor)
        {
            _jwtManager = manager;
            _cookieService = cookieService;
            _queryHandler = queryHandler;
            _auditLogger = auditLogger;
            _actor = actor;
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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest req, CancellationToken ct)
        {

            string token = await _jwtManager.MakeToken(req.Email, req.Password, ct);
            _cookieService.SetAccessToken(Response, token);

            await _auditLogger.LogAsync(new AuditLogEntry
            {
                ActorEmail = req.Email,
                ActorId = null,
                EntityId = null,
                EntityType = "User",
                EntityName = req.Email,
                EventType = AuditEventType.UserLoggedIn,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            return Ok(new SuccessResponse
            {
                Message = "Logged in",
                Data = null
            });
        }



        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            _cookieService.ClearAccessToken(Response);

            await _auditLogger.LogAsync(new AuditLogEntry
            {
                ActorEmail = _actor.Email,
                ActorId = _actor.Id,
                EntityId = _actor.Id,
                EntityType = "User",
                EntityName = _actor.Email,
                EventType = AuditEventType.UserLoggedOut,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            return StatusCode(204,  new SuccessResponse
            {
                Data = null,
                Message = "User successfully logged out"
            });
        }

    }
}

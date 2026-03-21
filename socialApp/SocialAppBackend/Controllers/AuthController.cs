using Microsoft.AspNetCore.Mvc;
using SocialAppBackend.Common;
using SocialAppBackend.Models;
using SocialAppBackend.Models.DTOs.Inbound;
using SocialAppBackend.Models.DTOs.Outbound;
using SocialAppBackend.Services;
namespace SocialAppBackend.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;

    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthService service, ILogger<AuthController> log)
    {
        _service = service;
        _logger = log;
    }
    // ================ POST / CREATE


    [HttpPost("register")]

    public async Task<ActionResult<RegisterResponseDto?>> Register([FromBody] RegisterDto dto)
    {
        // the service will reject if the username is taken
        var registered = await _service.RegisterAsync(dto.UserName, dto.Password);

        if (registered is null) return Conflict();
        
        return Ok(registered);
    }


    [HttpPost("login")]

    public async Task<ActionResult<LoginResponseDto?>> Login([FromBody] LoginDto dto)
    {
        var loggedIn = await _service.LoginAsync(dto.UserName, dto.Password);

        if (!loggedIn.Success)
        {
            return loggedIn.Error switch
            {
                ServiceError.InvalidCredentials => Unauthorized("invalid username and / or password"),
                _ => StatusCode(500)
            };
        }

        return Ok(loggedIn.Data);
    }

    // ================ GET / READ


    
}
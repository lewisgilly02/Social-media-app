using Microsoft.AspNetCore.Mvc;
using SocialAppBackend.Models;
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

        if (loggedIn is null) return Unauthorized();

        return Ok(loggedIn);
    }

    // ================ GET / READ


    
}
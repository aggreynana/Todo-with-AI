using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Todo.Model;
using Todo.Model.AuthDto;
using Todo.Services.Interfaces;

namespace Todo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto loginRequest)
    {
        _logger.LogInformation("Login request received for email: {Email}", loginRequest.Email);
        var response = await _authService.LoginAsync(loginRequest);
        if (response.StatusCode >= 200 && response.StatusCode < 300)
        {
            _logger.LogInformation("Login successful for email: {Email}", loginRequest.Email);
        }
        else
        {
            _logger.LogWarning("Login failed for email: {Email}", loginRequest.Email);
        }
        return StatusCode(Response.StatusCode, response);
    }
}

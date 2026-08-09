using Microsoft.AspNetCore.Mvc;
using Todo.Model;
using Todo.Model.AuthDto;
using Todo.Services.Interfaces;

namespace Todo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto loginRequest)
    {
        var response = await _authService.LoginAsync(loginRequest);
        return StatusCode(Response.StatusCode, response);
    }
}

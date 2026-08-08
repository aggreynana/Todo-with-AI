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
        var result = await _authService.LoginAsync(loginRequest);
        
        if (result.StatusCode == 200)
            return Ok(result);
        
        if (result.StatusCode == 401)
            return Unauthorized(result);
        
        return BadRequest(result);
    }
}

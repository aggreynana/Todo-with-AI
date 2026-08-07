using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Model.AuthDto;
using Todo.Services.Interfaces;

namespace Todo.Controllers;

// STEP 1: Create the AuthController for authentication endpoints
// This controller handles user login and token generation
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // STEP 2: Inject the IAuthService through constructor injection
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // STEP 3: Create the login endpoint
    // This endpoint allows users to authenticate and receive a JWT token
    [HttpPost("login")]
    [AllowAnonymous] // STEP 4: Allow anonymous access for login endpoint
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
    {
        // STEP 5: Validate the request model
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // STEP 6: Call the authentication service to validate credentials and generate token
        var result = await _authService.LoginAsync(loginRequest);

        // STEP 7: Return the appropriate response based on the result
        if (result.StatusCode == 200)
        {
            return Ok(result);
        }

        // STEP 8: Return error response with appropriate status code
        return StatusCode(result.StatusCode, result);
    }
}

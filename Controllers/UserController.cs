using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Todo.Model;
using Todo.Model.FilterDto;
using Todo.Model.UserDto;
using Todo.Services.Interfaces;

namespace Todo.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> CreateUser([FromBody] CreateUserRequestDto userDto)
    {
        _logger.LogInformation("Creating new user with email: {Email}", userDto.Email);
        var response = await _userService.CreateUserAsync(userDto);
        if (response.StatusCode >= 200 && response.StatusCode < 300)
        {
            _logger.LogInformation("User created successfully with ID: {UserId}", response?.Data?.UserId);
        }
        else
        {
            _logger.LogWarning("User creation failed for email: {Email}", userDto.Email);
        }
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetUserResponseDto>>> GetUserById(string id)
    {
        _logger.LogInformation("Fetching user with ID: {UserId}", id);
        var response = await _userService.GetUserByIdAsync(id);
        if (response == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", id);
            return NotFound(ApiResponse<GetUserResponseDto>.NotFound("User not found"));
        }
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PageResultResponseDto<GetUserResponseDto>>>> GetUsers([FromQuery] UserFilterDto? userFilter = null)
    {
        _logger.LogInformation("Fetching users with filter: {@Filter}", userFilter);
        var response = await _userService.GetUsersAsync(userFilter);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<GetUserResponseDto>>> UpdateUser(string id, [FromBody] UpdateUserRequestDto userUpdate)
    {
        _logger.LogInformation("Updating user with ID: {UserId}", id);
        var response = await _userService.UpdateUserAsync(id, userUpdate);
        if (response.StatusCode >= 200 && response.StatusCode < 300)
        {
            _logger.LogInformation("User updated successfully with ID: {UserId}", id);
        }
        else
        {
            _logger.LogWarning("User update failed for ID: {UserId}", id);
        }
        return StatusCode(Response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(string id)
    {
        _logger.LogInformation("Deleting user with ID: {UserId}", id);
        var response = await _userService.DeleteUserByIdAsync(id);
        if (response.StatusCode >= 200 && response.StatusCode < 300)
        {
            _logger.LogInformation("User deleted successfully with ID: {UserId}", id);
        }
        else
        {
            _logger.LogWarning("User deletion failed for ID: {UserId}", id);
        }
        return StatusCode(Response.StatusCode, response);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> CreateUser([FromBody] CreateUserRequestDto userDto)
    {
        var response = await _userService.CreateUserAsync(userDto);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetUserResponseDto>>> GetUserById(string id)
    {
        var response = await _userService.GetUserByIdAsync(id);
        if (response == null)
            return NotFound(ApiResponse<GetUserResponseDto>.NotFound("User not found"));
        return StatusCode(Response.StatusCode, response);
    }


    [HttpGet]
    public async Task<ActionResult<ApiResponse<PageResultResponseDto<GetUserResponseDto>>>> GetUsers([FromQuery] UserFilterDto? userFilter = null)
    {
        var response = await _userService.GetUsersAsync(userFilter);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<GetUserResponseDto>>> UpdateUser(string id, [FromBody] UpdateUserRequestDto userUpdate)
    {
        var response = await _userService.UpdateUserAsync(id, userUpdate);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(string id)
    {
        var response = await _userService.DeleteUserByIdAsync(id);
        return StatusCode(Response.StatusCode, response);
    }
}

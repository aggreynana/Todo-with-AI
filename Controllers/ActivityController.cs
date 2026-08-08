using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Model;
using Todo.Model.ActivityDto;
using Todo.Model.FilterDto;
using Todo.Services.Interfaces;

namespace Todo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ActivityController : ControllerBase
{
    private readonly IActivityService _activityService;

    public ActivityController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GetActivityResponseDto>>> CreateActivity([FromBody] CreateActivityRequestDto activityDto)
    {
        var response = await _activityService.CreateActivityAsync(activityDto);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetActivityResponseDto>>> GetActivityById(string id)
    {
        var response = await _activityService.GetActivityByIdAsync(id);
        if (response == null)
            return NotFound(ApiResponse<GetActivityResponseDto>.NotFound("Activity not found"));
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PageResultResponseDto<GetActivityResponseDto>>>> GetActivities([FromQuery] ActivityFilterDto? activityFilter = null)
    {
        var response = await _activityService.GetActivitiesAsync(activityFilter);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<GetActivityResponseDto>>> UpdateActivity(string id, [FromBody] UpdateActivityRequestDto activityUpdate)
    {
        var response = await _activityService.UpdateActivityAsync(id, activityUpdate);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteActivity(string id)
    {
        var response = await _activityService.DeleteActivityByIdAsync(id);
        return StatusCode(Response.StatusCode, response);
    }
}

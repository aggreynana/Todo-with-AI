using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Todo.Model;
using Todo.Model.ActivityDto;
using Todo.Model.FilterDto;
using Todo.Services.Interfaces;

namespace Todo.Controllers;

/// <summary>
/// Controller for managing activity CRUD operations
/// Requires authorization for all endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ActivityController : ControllerBase
{
    private readonly IActivityService _activityService;
    // Logger for tracking activity operations and debugging
    private readonly ILogger<ActivityController> _logger;

    public ActivityController(IActivityService activityService, ILogger<ActivityController> logger)
    {
        _activityService = activityService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new activity
    /// </summary>
    /// <param name="activityDto">The activity data to create</param>
    /// <returns>The created activity response</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<GetActivityResponseDto>>> CreateActivity([FromBody] CreateActivityRequestDto activityDto)
    {
        // Log the creation attempt with the activity title for tracking
        _logger.LogInformation("Creating new activity with title: {Title}", activityDto.Title);
        var response = await _activityService.CreateActivityAsync(activityDto);
        // Log successful creation with the generated activity ID
        _logger.LogInformation("Activity created successfully with ID: {ActivityId}", response?.Data?.Id);
        return StatusCode(Response.StatusCode, response);
    }

    /// <summary>
    /// Retrieves a specific activity by its ID
    /// </summary>
    /// <param name="id">The activity ID to retrieve</param>
    /// <returns>The activity details or 404 if not found</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetActivityResponseDto>>> GetActivityById(string id)
    {
        // Log the retrieval attempt with the activity ID
        _logger.LogInformation("Fetching activity with ID: {ActivityId}", id);
        var response = await _activityService.GetActivityByIdAsync(id);
        if (response == null)
        {
            // Log a warning when the requested activity is not found
            _logger.LogWarning("Activity not found with ID: {ActivityId}", id);
            return NotFound(ApiResponse<GetActivityResponseDto>.NotFound("Activity not found"));
        }
        return StatusCode(Response.StatusCode, response);
    }

    /// <summary>
    /// Retrieves a paginated list of activities with optional filtering
    /// </summary>
    /// <param name="activityFilter">Optional filter criteria for the activities</param>
    /// <returns>Paginated list of activities</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PageResultResponseDto<GetActivityResponseDto>>>> GetActivities([FromQuery] ActivityFilterDto? activityFilter = null)
    {
        // Log the retrieval attempt with the filter parameters for debugging
        _logger.LogInformation("Fetching activities with filter: {@Filter}", activityFilter);
        var response = await _activityService.GetActivitiesAsync(activityFilter);
        return StatusCode(Response.StatusCode, response);
    }

    /// <summary>
    /// Updates an existing activity
    /// </summary>
    /// <param name="id">The activity ID to update</param>
    /// <param name="activityUpdate">The updated activity data</param>
    /// <returns>The updated activity response</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<GetActivityResponseDto>>> UpdateActivity(string id, [FromBody] UpdateActivityRequestDto activityUpdate)
    {
        // Log the update attempt with activity ID and new title
        _logger.LogInformation("Updating activity with ID: {ActivityId}, new title: {Title}", id, activityUpdate.Title);
        var response = await _activityService.UpdateActivityAsync(id, activityUpdate);
        // Log successful update completion
        _logger.LogInformation("Activity updated successfully with ID: {ActivityId}", id);
        return StatusCode(Response.StatusCode, response);
    }

    /// <summary>
    /// Deletes an activity by its ID
    /// </summary>
    /// <param name="id">The activity ID to delete</param>
    /// <returns>Success response indicating deletion</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteActivity(string id)
    {
        // Log the deletion attempt with the activity ID
        _logger.LogInformation("Deleting activity with ID: {ActivityId}", id);
        var response = await _activityService.DeleteActivityByIdAsync(id);
        // Log successful deletion completion
        _logger.LogInformation("Activity deleted successfully with ID: {ActivityId}", id);
        return StatusCode(Response.StatusCode, response);
    }
}

using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using Todo.Entities;
using Todo.Model;
using Todo.Model.ActivityDto;
using Todo.Model.FilterDto;
using Todo.Services.Interfaces;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Services.Providers;

public class ActivityService : IActivityService
{
    private readonly IActivityRepository _activityRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ActivityService> _logger;

    public ActivityService(IActivityRepository activityRepository, ICacheService cacheService, ILogger<ActivityService> logger)
    {
        _activityRepository = activityRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse<GetActivityResponseDto>> CreateActivityAsync(CreateActivityRequestDto activityDto)
    {
        _logger.LogInformation("Creating activity for user: {UserId}, title: {Title}", activityDto.UserId, activityDto.Title);
        var activity = await _activityRepository.GetActivityById(activityDto.UserId);

        if (activity != null)
        {
            _logger.LogWarning("Activity creation failed - activity already exists for user: {UserId}", activityDto.UserId);
            return ApiResponse<GetActivityResponseDto>.FailedDependency();
        }

        var activityEntity = new ActivityEntity
        {
            UserId = activityDto.UserId,
            Title = activityDto.Title,
            Description = activityDto.Description,
            Status = activityDto.Status,
            Priority = activityDto.Priority,
            CategoryId = activityDto.CategoryId,
            StartedOn = activityDto.StartedOn,
            EndedOn = activityDto.EndedOn
        };

        var result = await _activityRepository.AddActivityAsync(activityEntity);

        if (!result)
        {
            _logger.LogError("Activity creation failed for user: {UserId}", activityDto.UserId);
            return ApiResponse<GetActivityResponseDto>.InternalServerError();
        }

        var responseDto = MapToResponseDto(activityEntity);
        _logger.LogInformation("Activity created successfully with ID: {ActivityId}", activityEntity.Id);

        return ApiResponse<GetActivityResponseDto>.CreatedResponse("Activity", responseDto);
    }

    public async Task<ApiResponse<GetActivityResponseDto>?> GetActivityByIdAsync(string id)
    {
        _logger.LogInformation("Fetching activity with ID: {ActivityId}", id);
        // Try to get from cache first
        var cacheKey = $"activity_{id}";
        var cachedActivity = await _cacheService.GetAsync<GetActivityResponseDto>(cacheKey);

        if (cachedActivity != null)
        {
            _logger.LogInformation("Activity retrieved from cache with ID: {ActivityId}", id);
            return ApiResponse<GetActivityResponseDto>.OkResponse("Activity retrieved from cache", cachedActivity);
        }

        var activityEntity = await _activityRepository.GetActivityById(id);

        if (activityEntity == null)
        {
            _logger.LogWarning("Activity not found with ID: {ActivityId}", id);
            return ApiResponse<GetActivityResponseDto>.NoContent();
        }

        // Map the entity to DTO
        var responseDto = MapToResponseDto(activityEntity);

        // Cache the result for 5 minutes
        await _cacheService.SetAsync(cacheKey, responseDto, TimeSpan.FromMinutes(5));

        _logger.LogInformation("Activity retrieved successfully with ID: {ActivityId}", id);
        // STEP 15: Return the activity in a success response
        return ApiResponse<GetActivityResponseDto>.OkResponse("Activity retrieved successfully", responseDto);
    }

    public async Task<ApiResponse<PageResultResponseDto<GetActivityResponseDto>>> GetActivitiesAsync(ActivityFilterDto? activityFilter = null)
    {
        _logger.LogInformation("Fetching activities with filter: {@Filter}", activityFilter);
        // Use default filter if not provided
        var filter = activityFilter ?? new ActivityFilterDto();

        // Call the repository with filter
        var activityPageResult = await _activityRepository.GetActivitiesWithPaginationAsync(filter);

        var activityDtos = activityPageResult.Records.Select(MapToResponseDto).ToList();

        var pageResult = new PageResultResponseDto<GetActivityResponseDto>
        {
            Page = activityPageResult.Page,
            PageSize = activityPageResult.PageSize,
            Records = activityDtos,
            TotalCount = activityPageResult.TotalCount,
            TotalPages = activityPageResult.TotalPages
        };

        _logger.LogInformation("Retrieved {Count} activities", activityDtos.Count);
        return ApiResponse<PageResultResponseDto<GetActivityResponseDto>>.OkResponse("Activities retrieved successfully", pageResult);
    }


    public async Task<ApiResponse<GetActivityResponseDto>> UpdateActivityAsync(string id, UpdateActivityRequestDto activityUpdate)
    {
        _logger.LogInformation("Updating activity with ID: {ActivityId}", id);

        var existingActivity = await _activityRepository.GetActivityById(id);

        if (existingActivity == null)
        {
            _logger.LogWarning("Activity not found for update with ID: {ActivityId}", id);
            return ApiResponse<GetActivityResponseDto>.NotFound("Activity not found");
        }

        if (activityUpdate.Status.HasValue)
            existingActivity.Status = activityUpdate.Status.Value;
        if (activityUpdate.Priority.HasValue)
            existingActivity.Priority = activityUpdate.Priority.Value;

        existingActivity.Title = activityUpdate.Title ?? string.Empty;
        existingActivity.Description = activityUpdate.Description;
        existingActivity.CategoryId = activityUpdate.CategoryId ?? string.Empty;
        existingActivity.StartedOn = activityUpdate.StartedOn;
        existingActivity.EndedOn = activityUpdate.EndedOn;
        existingActivity.ModifiedOn = DateTime.UtcNow;

        var result = await _activityRepository.UpdateActivityAsync(existingActivity);

        if (!result)
        {
            _logger.LogError("Activity update failed for ID: {ActivityId}", id);
            return ApiResponse<GetActivityResponseDto>.InternalServerError();
        }

        var responseDto = MapToResponseDto(existingActivity);

        // Invalidate cache for this activity
        var cacheKey = $"activity_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        _logger.LogInformation("Activity updated successfully with ID: {ActivityId}", id);
        return ApiResponse<GetActivityResponseDto>.AcceptedResponse();
    }


    public async Task<ApiResponse<bool>> DeleteActivityByIdAsync(string id)
    {
        _logger.LogInformation("Deleting activity with ID: {ActivityId}", id);

        var existingActivity = await _activityRepository.GetActivityById(id);

        if (existingActivity == null)
        {
            _logger.LogWarning("Activity not found for deletion with ID: {ActivityId}", id);
            return ApiResponse<bool>.NotFound("Activity not found");
        }

        var isActivityDeleted = await _activityRepository.DeleteActivityByIdAsync(existingActivity);

        if (!isActivityDeleted)
        {
            _logger.LogError("Activity deletion failed for ID: {ActivityId}", id);
            return ApiResponse<bool>.InternalServerError();
        }

        // Invalidate cache for this activity
        var cacheKey = $"activity_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        _logger.LogInformation("Activity deleted successfully with ID: {ActivityId}", id);
        return ApiResponse<bool>.NoContent();
    }

    // STEP 43: Create a helper method to map Entity to DTO
    // This private method reduces code duplication and ensures consistent mapping
    private static GetActivityResponseDto MapToResponseDto(ActivityEntity entity)
    {
        return new GetActivityResponseDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Title = entity.Title,
            Description = entity.Description,
            Status = entity.Status,
            Priority = entity.Priority,
            CategoryId = entity.CategoryId,
            CreatedOn = entity.CreatedOn,
            ModifiedOn = entity.ModifiedOn,
            StartedOn = entity.StartedOn,
            EndedOn = entity.EndedOn,
            Duration = entity.Duration
        };
    }
}

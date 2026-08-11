using System.Linq;
using System;
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

    public ActivityService(IActivityRepository activityRepository, ICacheService cacheService)
    {
        _activityRepository = activityRepository;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<GetActivityResponseDto>> CreateActivityAsync(CreateActivityRequestDto activityDto)
    {
        var activity = await _activityRepository.GetActivityById(activityDto.UserId);

        if (activity != null) return ApiResponse<GetActivityResponseDto>.FailedDependency();
        
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
            return ApiResponse<GetActivityResponseDto>.InternalServerError();
        }

        var responseDto = MapToResponseDto(activityEntity);

        return ApiResponse<GetActivityResponseDto>.CreatedResponse("Activity", responseDto);
    }

    public async Task<ApiResponse<GetActivityResponseDto>?> GetActivityByIdAsync(string id)
    {
        // Try to get from cache first
        var cacheKey = $"activity_{id}";
        var cachedActivity = await _cacheService.GetAsync<GetActivityResponseDto>(cacheKey);
        
        if (cachedActivity != null)
        {
            return ApiResponse<GetActivityResponseDto>.OkResponse("Activity retrieved from cache", cachedActivity);
        }

        var activityEntity = await _activityRepository.GetActivityById(id);

        if (activityEntity == null)
        {
            return ApiResponse<GetActivityResponseDto>.NoContent();
        }

        // Map the entity to DTO
        var responseDto = MapToResponseDto(activityEntity);

        // Cache the result for 5 minutes
        await _cacheService.SetAsync(cacheKey, responseDto, TimeSpan.FromMinutes(5));

        // STEP 15: Return the activity in a success response
        return ApiResponse<GetActivityResponseDto>.OkResponse("Activity retrieved successfully", responseDto);
    }

    public async Task<ApiResponse<PageResultResponseDto<GetActivityResponseDto>>> GetActivitiesAsync(ActivityFilterDto? activityFilter = null)
    {
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

        return ApiResponse<PageResultResponseDto<GetActivityResponseDto>>.OkResponse("Activities retrieved successfully", pageResult);
    }

    // STEP 24: Implement UpdateActivityAsync method
    // This method updates an existing activity
    public async Task<ApiResponse<GetActivityResponseDto>> UpdateActivityAsync(string id, UpdateActivityRequestDto activityUpdate)
    {
        // STEP 25: First, retrieve the existing activity
        var existingActivity = await _activityRepository.GetActivityById(id);

        // STEP 26: Check if the activity exists
        if (existingActivity == null)
        {
            // STEP 27: Return an error response if activity not found
            return ApiResponse<GetActivityResponseDto>.NotFound("Activity not found");
        }

        // STEP 28: Update only the fields that are provided (partial update)
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
            return ApiResponse<GetActivityResponseDto>.InternalServerError();
        }

        
        var responseDto = MapToResponseDto(existingActivity);

        // Invalidate cache for this activity
        var cacheKey = $"activity_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        // STEP 34: Return a success response with the updated activity
        return ApiResponse<GetActivityResponseDto>.AcceptedResponse();
    }

    // STEP 35: Implement DeleteActivityByIdAsync method
    // This method deletes an activity by its ID
    public async Task<ApiResponse<bool>> DeleteActivityByIdAsync(string id)
    {
        // STEP 36: First, retrieve the existing activity
        var existingActivity = await _activityRepository.GetActivityById(id);

        // STEP 37: Check if the activity exists
        if (existingActivity == null)
        {
            // STEP 38: Return an error response if activity not found
            return ApiResponse<bool>.NotFound("Activity not found");
        }

        // STEP 39: Call the repository to delete the entity
        var isActivityDeleted = await _activityRepository.DeleteActivityByIdAsync(existingActivity);

        // STEP 40: Check if the deletion was successful
        if (!isActivityDeleted)
        {
            // STEP 41: Return an error response if deletion failed
            return ApiResponse<bool>.InternalServerError();
        }

        // Invalidate cache for this activity
        var cacheKey = $"activity_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        // STEP 42: Return a success response indicating successful deletion
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

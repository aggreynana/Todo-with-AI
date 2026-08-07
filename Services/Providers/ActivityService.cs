using System.Linq;
using System;
using Todo.Entities;
using Todo.Model;
using Todo.Model.ActivityDto;
using Todo.Model.FilterDto;
using Todo.Services.Interfaces;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Services.Providers;

// STEP 1: Implement the IActivityService interface
// This class contains the business logic for Activity operations
// It acts as a bridge between the controller and the repository layer
public class ActivityService : IActivityService
{
    // STEP 2: Inject the IActivityRepository through constructor injection
    // This follows the dependency injection pattern for loose coupling
    private readonly IActivityRepository _activityRepository;

    public ActivityService(IActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    // STEP 3: Implement CreateActivityAsync method
    // This method handles the creation of a new activity
    public async Task<ApiResponse<GetActivityResponseDto>> CreateActivityAsync(CreateActivityRequestDto activityDto)
    {
        // STEP 4: Map the DTO to the entity
        // Convert the incoming DTO to the domain entity for database operations
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
            // Id and CreatedOn are set automatically in BaseEntity
        };

        // STEP 5: Call the repository to add the entity to the database
        var result = await _activityRepository.AddActivityAsync(activityEntity);

        // STEP 6: Check if the operation was successful
        if (!result)
        {
            // STEP 7: Return an error response if creation failed
            return ApiResponse<GetActivityResponseDto>.InternalServerError();
        }

        // STEP 8: Map the entity back to DTO for the response
        var responseDto = MapToResponseDto(activityEntity);

        // STEP 9: Return a success response with the created activity
        return ApiResponse<GetActivityResponseDto>.CreatedResponse("Activity", responseDto);
    }

    // STEP 10: Implement GetActivityByIdAsync method
    // This method retrieves a single activity by its unique identifier
    public async Task<ApiResponse<GetActivityResponseDto>?> GetActivityByIdAsync(string id)
    {
        // STEP 11: Call the repository to get the activity
        var activityEntity = await _activityRepository.GetActivityById(id);

        // STEP 12: Check if the activity exists
        if (activityEntity == null)
        {
            // STEP 13: Return null if activity not found
            return null;
        }

        // STEP 14: Map the entity to DTO
        var responseDto = MapToResponseDto(activityEntity);

        // STEP 15: Return the activity in a success response
        return ApiResponse<GetActivityResponseDto>.OkResponse("Activity retrieved successfully", responseDto);
    }

    // Implement GetActivitiesAsync method with pagination and filtering
    // This method retrieves activities from the database with pagination and filtering support
    public async Task<ApiResponse<PageResultResponseDto<GetActivityResponseDto>>> GetActivitiesAsync(ActivityFilterDto? activityFilter = null)
    {
        // Use default filter if not provided
        var filter = activityFilter ?? new ActivityFilterDto();

        // Call the repository with filter
        var activityPageResult = await _activityRepository.GetActivitiesWithPaginationAsync(filter);

        // Map entities to DTOs
        var activityDtos = activityPageResult.Records.Select(MapToResponseDto).ToList();

        // Create paginated response with metadata
        var pageResult = new PageResultResponseDto<GetActivityResponseDto>
        {
            Page = activityPageResult.Page,
            PageSize = activityPageResult.PageSize,
            Records = activityDtos,
            TotalCount = activityPageResult.TotalCount,
            TotalPages = activityPageResult.TotalPages
        };

        // Return paginated response
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
        if (activityUpdate.Title != null)
            existingActivity.Title = activityUpdate.Title;
        if (activityUpdate.Description != null)
            existingActivity.Description = activityUpdate.Description;
        if (activityUpdate.Status.HasValue)
            existingActivity.Status = activityUpdate.Status.Value;
        if (activityUpdate.Priority.HasValue)
            existingActivity.Priority = activityUpdate.Priority.Value;
        if (activityUpdate.CategoryId != null)
            existingActivity.CategoryId = activityUpdate.CategoryId;
        if (activityUpdate.StartedOn.HasValue)
            existingActivity.StartedOn = activityUpdate.StartedOn;
        if (activityUpdate.EndedOn.HasValue)
            existingActivity.EndedOn = activityUpdate.EndedOn;

        // STEP 29: Set the ModifiedOn timestamp
        existingActivity.ModifiedOn = DateTime.UtcNow;

        // STEP 30: Call the repository to update the entity
        var result = await _activityRepository.UpdateActivityAsync(existingActivity);

        // STEP 31: Check if the update was successful
        if (!result)
        {
            // STEP 32: Return an error response if update failed
            return ApiResponse<GetActivityResponseDto>.InternalServerError();
        }

        // STEP 33: Map the updated entity to DTO
        var responseDto = MapToResponseDto(existingActivity);

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
        var result = await _activityRepository.DeleteActivityByIdAsync(existingActivity);

        // STEP 40: Check if the deletion was successful
        if (!result)
        {
            // STEP 41: Return an error response if deletion failed
            return ApiResponse<bool>.InternalServerError();
        }

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

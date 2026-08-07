using Todo.Model;
using Todo.Model.ActivityDto;
using Todo.Model.FilterDto;

namespace Todo.Services.Interfaces;

// Define the interface for Activity service
// This interface defines the contract for all business logic operations related to Activity entities
// It follows the dependency inversion principle - high-level modules depend on abstractions, not concrete implementations
public interface IActivityService
{
    // Create method for adding a new activity
    // Takes a DTO (Data Transfer Object) as input to create a new activity
    // Returns a wrapped response with the created activity DTO
    Task<ApiResponse<GetActivityResponseDto>> CreateActivityAsync(CreateActivityRequestDto activityDto);

    // Create method to retrieve a single activity by its ID
    // Returns nullable response since the activity might not exist
    Task<ApiResponse<GetActivityResponseDto>?> GetActivityByIdAsync(string id);

    // Create method to retrieve all activities with pagination and filtering
    // Returns a wrapped response with paginated activity DTOs
    Task<ApiResponse<PageResultResponseDto<GetActivityResponseDto>>> GetActivitiesAsync(ActivityFilterDto? activityFilter = null);

    // Create method to update an existing activity
    // Takes an update DTO and returns the updated activity DTO
    Task<ApiResponse<GetActivityResponseDto>> UpdateActivityAsync(string id, UpdateActivityRequestDto activityUpdate);

    // Create method to delete an activity by its ID
    // Returns a boolean indicating success/failure
    Task<ApiResponse<bool>> DeleteActivityByIdAsync(string id);
}

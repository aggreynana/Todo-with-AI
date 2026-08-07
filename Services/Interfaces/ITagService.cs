using Todo.Model;
using Todo.Model.FilterDto;
using Todo.Model.TagDto;

namespace Todo.Services.Interfaces;

// Define the interface for Tag service
// This interface defines the contract for all business logic operations related to Tag entities
public interface ITagService
{
    // Create method for adding a new tag
    // Takes a DTO as input to create a new tag
    // Returns a wrapped response with the created tag DTO
    Task<ApiResponse<GetTagResponseDto>> CreateTagAsync(CreateTagRequestDto tagDto);

    // Create method to retrieve a single tag by its ID
    // Returns nullable response since the tag might not exist
    Task<ApiResponse<GetTagResponseDto>?> GetTagByIdAsync(string id);

    // Create method to retrieve all tags with pagination and filtering
    // Returns a wrapped response with paginated tag DTOs
    Task<ApiResponse<PageResultResponseDto<GetTagResponseDto>>> GetTagsAsync(TagFilterDto? tagFilter = null);

    // Create method to update an existing tag
    // Takes an update DTO and returns the updated tag DTO
    Task<ApiResponse<GetTagResponseDto>> UpdateTagAsync(string id, UpdateTagRequestDto tagUpdate);

    // Create method to delete a tag by its ID
    // Returns a boolean indicating success/failure
    Task<ApiResponse<bool>> DeleteTagByIdAsync(string id);
}

using Todo.Model;
using Todo.Model.CategoryDto;
using Todo.Model.FilterDto;

namespace Todo.Services.Interfaces;

// Define the interface for Category service
// This interface defines the contract for all business logic operations related to Category entities
public interface ICategoryService
{
    // Create method for adding a new category
    // Takes a DTO as input to create a new category
    // Returns a wrapped response with the created category DTO
    Task<ApiResponse<GetCategoryResponseDto>> CreateCategoryAsync(CreateCategoryRequestDto categoryDto);

    // Create method to retrieve a single category by its ID
    // Returns nullable response since the category might not exist
    Task<ApiResponse<GetCategoryResponseDto>?> GetCategoryByIdAsync(string id);

    // Create method to retrieve all categories with pagination and filtering
    // Returns a wrapped response with paginated category DTOs
    Task<ApiResponse<PageResultResponseDto<GetCategoryResponseDto>>> GetCategoriesAsync(CategoryFilterDto? categoryFilter = null);

    // Create method to update an existing category
    // Takes an update DTO and returns the updated category DTO
    Task<ApiResponse<GetCategoryResponseDto>> UpdateCategoryAsync(string id, UpdateCategoryRequestDto categoryUpdate);

    // Create method to delete a category by its ID
    // Returns a boolean indicating success/failure
    Task<ApiResponse<bool>> DeleteCategoryByIdAsync(string id);
}

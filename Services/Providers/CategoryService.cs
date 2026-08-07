using System.Linq;
using System;
using Todo.Entities;
using Todo.Model;
using Todo.Model.CategoryDto;
using Todo.Model.FilterDto;
using Todo.Services.Interfaces;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Services.Providers;

// STEP 1: Implement the ICategoryService interface
// This class contains the business logic for Category operations
// It acts as a bridge between the controller and the repository layer
public class CategoryService : ICategoryService
{
    // STEP 2: Inject the ICategoryRepository through constructor injection
    // This follows the dependency injection pattern for loose coupling
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    // STEP 3: Implement CreateCategoryAsync method
    // This method handles the creation of a new category
    public async Task<ApiResponse<GetCategoryResponseDto>> CreateCategoryAsync(CreateCategoryRequestDto categoryDto)
    {
        // STEP 4: Map the DTO to the entity
        // Convert the incoming DTO to the domain entity for database operations
        var categoryEntity = new CategoryEntity
        {
            UserId = categoryDto.UserId,
            Name = categoryDto.Name
            // Id and CreatedOn are set automatically in BaseEntity
        };

        // STEP 5: Call the repository to add the entity to the database
        var result = await _categoryRepository.AddCategoryAsync(categoryEntity);

        // STEP 6: Check if the operation was successful
        if (!result)
        {
            // STEP 7: Return an error response if creation failed
            return ApiResponse<GetCategoryResponseDto>.InternalServerError();
        }

        // STEP 8: Map the entity back to DTO for the response
        var responseDto = MapToResponseDto(categoryEntity);

        // STEP 9: Return a success response with the created category
        return ApiResponse<GetCategoryResponseDto>.CreatedResponse("Category", responseDto);
    }

    // STEP 10: Implement GetCategoryByIdAsync method
    // This method retrieves a single category by its unique identifier
    public async Task<ApiResponse<GetCategoryResponseDto>?> GetCategoryByIdAsync(string id)
    {
        // STEP 11: Call the repository to get the category
        var categoryEntity = await _categoryRepository.GetCategoryById(id);

        // STEP 12: Check if the category exists
        if (categoryEntity == null)
        {
            // STEP 13: Return null if category not found
            return null;
        }

        // STEP 14: Map the entity to DTO
        var responseDto = MapToResponseDto(categoryEntity);

        // STEP 15: Return the category in a success response
        return ApiResponse<GetCategoryResponseDto>.OkResponse("Category retrieved successfully", responseDto);
    }

    // Implement GetCategoriesAsync method with pagination and filtering
    // This method retrieves categories from the database with pagination and filtering support
    public async Task<ApiResponse<PageResultResponseDto<GetCategoryResponseDto>>> GetCategoriesAsync(CategoryFilterDto? categoryFilter = null)
    {
        // Use default filter if not provided
        var filter = categoryFilter ?? new CategoryFilterDto();

        // Call the repository with filter
        var categoryPageResult = await _categoryRepository.GetCategoriesWithPaginationAsync(filter);

        // Map entities to DTOs
        var categoryDtos = categoryPageResult.Records.Select(MapToResponseDto).ToList();

        // Create paginated response with metadata
        var pageResult = new PageResultResponseDto<GetCategoryResponseDto>
        {
            Page = categoryPageResult.Page,
            PageSize = categoryPageResult.PageSize,
            Records = categoryDtos,
            TotalCount = categoryPageResult.TotalCount,
            TotalPages = categoryPageResult.TotalPages
        };

        // Return paginated response
        return ApiResponse<PageResultResponseDto<GetCategoryResponseDto>>.OkResponse("Categories retrieved successfully", pageResult);
    }

    // STEP 24: Implement UpdateCategoryAsync method
    // This method updates an existing category
    public async Task<ApiResponse<GetCategoryResponseDto>> UpdateCategoryAsync(string id, UpdateCategoryRequestDto categoryUpdate)
    {
        // STEP 25: First, retrieve the existing category
        var existingCategory = await _categoryRepository.GetCategoryById(id);

        // STEP 26: Check if the category exists
        if (existingCategory == null)
        {
            // STEP 27: Return an error response if category not found
            return ApiResponse<GetCategoryResponseDto>.NotFound("Category not found");
        }

        // STEP 28: Update only the fields that are provided (partial update)
        if (categoryUpdate.Name != null)
            existingCategory.Name = categoryUpdate.Name;

        // STEP 29: Set the ModifiedOn timestamp
        existingCategory.ModifiedOn = DateTime.UtcNow;

        // STEP 30: Call the repository to update the entity
        var result = await _categoryRepository.UpdateCategoryAsync(existingCategory);

        // STEP 31: Check if the update was successful
        if (!result)
        {
            // STEP 32: Return an error response if update failed
            return ApiResponse<GetCategoryResponseDto>.InternalServerError();
        }

        // STEP 33: Map the updated entity to DTO
        var responseDto = MapToResponseDto(existingCategory);

        // STEP 34: Return a success response with the updated category
        return ApiResponse<GetCategoryResponseDto>.AcceptedResponse();
    }

    // STEP 35: Implement DeleteCategoryByIdAsync method
    // This method deletes a category by its ID
    public async Task<ApiResponse<bool>> DeleteCategoryByIdAsync(string id)
    {
        // STEP 36: First, retrieve the existing category
        var existingCategory = await _categoryRepository.GetCategoryById(id);

        // STEP 37: Check if the category exists
        if (existingCategory == null)
        {
            // STEP 38: Return an error response if category not found
            return ApiResponse<bool>.NotFound("Category not found");
        }

        // STEP 39: Call the repository to delete the entity
        var result = await _categoryRepository.DeleteCategoryByIdAsync(existingCategory);

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
    private static GetCategoryResponseDto MapToResponseDto(CategoryEntity entity)
    {
        return new GetCategoryResponseDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Name = entity.Name,
            CreatedOn = entity.CreatedOn,
            ModifiedOn = entity.ModifiedOn
        };
    }
}

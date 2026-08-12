using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using Todo.Entities;
using Todo.Model;
using Todo.Model.CategoryDto;
using Todo.Model.FilterDto;
using Todo.Services.Interfaces;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Services.Providers;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository categoryRepository, ICacheService cacheService, ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse<GetCategoryResponseDto>> CreateCategoryAsync(CreateCategoryRequestDto categoryDto)
    {
        _logger.LogInformation("Creating category for user: {UserId}, name: {Name}", categoryDto.UserId, categoryDto.Name);
        var category = await _categoryRepository.GetCategoryById(categoryDto.UserId);

        if (category != null)
        {
            _logger.LogWarning("Category creation failed - category already exists for user: {UserId}", categoryDto.UserId);
            return ApiResponse<GetCategoryResponseDto>.FailedDependency();
        }

        var categoryEntity = new CategoryEntity
        {
            UserId = categoryDto.UserId,
            Name = categoryDto.Name
        };

        var result = await _categoryRepository.AddCategoryAsync(categoryEntity);

        // Check if the operation was successful
        if (!result)
        {
            _logger.LogError("Category creation failed for user: {UserId}", categoryDto.UserId);
            return ApiResponse<GetCategoryResponseDto>.InternalServerError();
        }

        // Map the entity back to DTO for the response
        var responseDto = MapToResponseDto(categoryEntity);
        _logger.LogInformation("Category created successfully with ID: {CategoryId}", categoryEntity.Id);

        return ApiResponse<GetCategoryResponseDto>.CreatedResponse("Category", responseDto);
    }

    public async Task<ApiResponse<GetCategoryResponseDto>?> GetCategoryByIdAsync(string id)
    {
        _logger.LogInformation("Fetching category with ID: {CategoryId}", id);
        // Try to get from cache first
        var cacheKey = $"category_{id}";
        var cachedCategory = await _cacheService.GetAsync<GetCategoryResponseDto>(cacheKey);

        if (cachedCategory != null)
        {
            _logger.LogInformation("Category retrieved from cache with ID: {CategoryId}", id);
            return ApiResponse<GetCategoryResponseDto>.OkResponse("Category retrieved from cache", cachedCategory);
        }

        var categoryEntity = await _categoryRepository.GetCategoryById(id);

        if (categoryEntity == null)
        {
            _logger.LogWarning("Category not found with ID: {CategoryId}", id);
            return ApiResponse<GetCategoryResponseDto>.InternalServerError();
        }

        // Map the entity to DTO
        var responseDto = MapToResponseDto(categoryEntity);

        // Cache the result for 5 minutes
        await _cacheService.SetAsync(cacheKey, responseDto, TimeSpan.FromMinutes(5));

        _logger.LogInformation("Category retrieved successfully with ID: {CategoryId}", id);
        return ApiResponse<GetCategoryResponseDto>.OkResponse("Category retrieved successfully", responseDto);
    }


    public async Task<ApiResponse<PageResultResponseDto<GetCategoryResponseDto>>> GetCategoriesAsync(CategoryFilterDto? categoryFilter = null)
    {
        _logger.LogInformation("Fetching categories with filter: {@Filter}", categoryFilter);
        // Use default filter if not provided
        var filter = categoryFilter ?? new CategoryFilterDto();

        var categoryPageResult = await _categoryRepository.GetCategoriesWithPaginationAsync(filter);

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

        _logger.LogInformation("Retrieved {Count} categories", categoryDtos.Count);
        return ApiResponse<PageResultResponseDto<GetCategoryResponseDto>>.OkResponse("Categories retrieved successfully", pageResult);
    }


    public async Task<ApiResponse<GetCategoryResponseDto>> UpdateCategoryAsync(string id, UpdateCategoryRequestDto categoryUpdate)
    {
        _logger.LogInformation("Updating category with ID: {CategoryId}", id);
        // STEP 25: First, retrieve the existing category
        var existingCategory = await _categoryRepository.GetCategoryById(id);

        if (existingCategory == null)
        {
            _logger.LogWarning("Category not found for update with ID: {CategoryId}", id);
            return ApiResponse<GetCategoryResponseDto>.NotFound("Category not found");
        }

        // STEP 28: Update only the fields that are provided (partial update)
        if (categoryUpdate.Name != null)
            existingCategory.Name = categoryUpdate.Name;

        // STEP 29: Set the ModifiedOn timestamp
        existingCategory.ModifiedOn = DateTime.UtcNow;

        // STEP 30: Call the repository to update the entity
        var result = await _categoryRepository.UpdateCategoryAsync(existingCategory);

        if (!result)
        {
            _logger.LogError("Category update failed for ID: {CategoryId}", id);
            return ApiResponse<GetCategoryResponseDto>.InternalServerError();
        }

        var responseDto = MapToResponseDto(existingCategory);

        // Invalidate cache for this category
        var cacheKey = $"category_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        _logger.LogInformation("Category updated successfully with ID: {CategoryId}", id);
        return ApiResponse<GetCategoryResponseDto>.OkResponse("Category updated Successfully", responseDto);
    }

    public async Task<ApiResponse<bool>> DeleteCategoryByIdAsync(string id)
    {
        _logger.LogInformation("Deleting category with ID: {CategoryId}", id);
        // STEP 36: First, retrieve the existing category
        var existingCategory = await _categoryRepository.GetCategoryById(id);

        if (existingCategory == null)
        {
            _logger.LogWarning("Category not found for deletion with ID: {CategoryId}", id);
            return ApiResponse<bool>.NotFound("Category not found");
        }

        var result = await _categoryRepository.DeleteCategoryByIdAsync(existingCategory);

        if (!result)
        {
            _logger.LogError("Category deletion failed for ID: {CategoryId}", id);
            return ApiResponse<bool>.InternalServerError();
        }

        // Invalidate cache for this category
        var cacheKey = $"category_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        _logger.LogInformation("Category deleted successfully with ID: {CategoryId}", id);
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

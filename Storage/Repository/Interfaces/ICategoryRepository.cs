using Todo.Entities;
using Todo.Model;
using Todo.Model.FilterDto;

namespace Todo.Storage.Repository.Interfaces;

public interface ICategoryRepository
{
    Task<bool> AddCategoryAsync(CategoryEntity category);
    Task<CategoryEntity?> GetCategoryById(string id);
    Task<bool> UpdateCategoryAsync(CategoryEntity category);
    Task<bool> DeleteCategoryByIdAsync(CategoryEntity category);

    // Pagination method using FilterDto approach
    // This method supports filtering, sorting, and pagination
    // Returns PageResultResponseDto with all pagination metadata
    Task<PageResultResponseDto<CategoryEntity>> GetCategoriesWithPaginationAsync(CategoryFilterDto categoryFilter);
}

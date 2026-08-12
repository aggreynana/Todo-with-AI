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

    Task<PageResultResponseDto<CategoryEntity>> GetCategoriesWithPaginationAsync(CategoryFilterDto categoryFilter);
}

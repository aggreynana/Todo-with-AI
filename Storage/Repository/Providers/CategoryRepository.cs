using Microsoft.EntityFrameworkCore;
using Todo.Entities;
using Todo.Enums;
using Todo.Model;
using Todo.Model.FilterDto;
using Todo.Storage.Context;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Storage.Repository.Providers;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;
    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddCategoryAsync(CategoryEntity category)
    {
        await _context.Categories.AddAsync(category);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteCategoryByIdAsync(CategoryEntity category)
    {
        _context.Categories.Remove(category);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<CategoryEntity?> GetCategoryById(string id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<bool> UpdateCategoryAsync(CategoryEntity category)
    {
        _context.Categories.Update(category);
        return await _context.SaveChangesAsync() > 0;
    }

    // Implement pagination method using FilterDto approach
    // This method implements filtering, sorting, and pagination
    public async Task<PageResultResponseDto<CategoryEntity>> GetCategoriesWithPaginationAsync(CategoryFilterDto categoryFilter)
    {
        var query = _context.Categories.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(categoryFilter.UserId))
        {
            query = query.Where(c => c.UserId == categoryFilter.UserId);
        }

        if (!string.IsNullOrWhiteSpace(categoryFilter.Name))
        {
            query = query.Where(c => c.Name.ToLower().Contains(categoryFilter.Name.ToLower()));
        }

        // Get total count
        var count = await query.CountAsync();

        // Create page result
        var pageResult = new PageResultResponseDto<CategoryEntity>
        {
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)categoryFilter.PageSize),
            Page = categoryFilter.Page,
            PageSize = categoryFilter.PageSize
        };

        var offset = (categoryFilter.Page - 1) * categoryFilter.PageSize;

        // Apply sorting and pagination
        if (categoryFilter.Sort == SortDirection.Desc)
        {
            pageResult.Records = await query.OrderByDescending(f => f.CreatedOn).Skip(offset)
                .Take(categoryFilter.PageSize).ToListAsync();
        }
        
        pageResult.Records = await query.OrderBy(f => f.CreatedOn).Skip(offset).Take(categoryFilter.PageSize).ToListAsync();

        return pageResult;
    }
}

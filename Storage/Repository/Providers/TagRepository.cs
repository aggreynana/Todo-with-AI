using Microsoft.EntityFrameworkCore;
using Todo.Entities;
using Todo.Enums;
using Todo.Model;
using Todo.Model.FilterDto;
using Todo.Storage.Context;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Storage.Repository.Providers;

public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _context;
    public TagRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddTagAsync(TagEntity tag)
    {
        await _context.Tags.AddAsync(tag);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteTagByIdAsync(TagEntity tag)
    {
        _context.Tags.Remove(tag);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<TagEntity?> GetTagById(string id)
    {
        return await _context.Tags.FindAsync(id);
    }

    public async Task<bool> UpdateTagAsync(TagEntity tag)
    {
        _context.Tags.Update(tag);
        return await _context.SaveChangesAsync() > 0;
    }

    // Implement pagination method using FilterDto approach
    // This method implements filtering, sorting, and pagination
    public async Task<PageResultResponseDto<TagEntity>> GetTagsWithPaginationAsync(TagFilterDto tagFilter)
    {
        var query = _context.Tags.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(tagFilter.UserId))
        {
            query = query.Where(t => t.UserId == tagFilter.UserId);
        }

        if (!string.IsNullOrWhiteSpace(tagFilter.Name))
        {
            query = query.Where(t => t.Name.ToLower().Contains(tagFilter.Name.ToLower()));
        }

        // Get total count
        var count = await query.CountAsync();

        // Create page result
        var pageResult = new PageResultResponseDto<TagEntity>
        {
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)tagFilter.PageSize),
            Page = tagFilter.Page,
            PageSize = tagFilter.PageSize
        };

        var offset = (tagFilter.Page - 1) * tagFilter.PageSize;

        // Apply sorting and pagination
        if (tagFilter.Sort == SortDirection.Desc)
        {
            pageResult.Records = await query.OrderByDescending(f => f.CreatedOn).Skip(offset)
                .Take(tagFilter.PageSize).ToListAsync();
        }
        
        pageResult.Records = await query.OrderBy(f => f.CreatedOn).Skip(offset).Take(tagFilter.PageSize).ToListAsync();

        return pageResult;
    }
}

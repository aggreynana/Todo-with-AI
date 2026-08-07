using Microsoft.EntityFrameworkCore;
using Todo.Entities;
using Todo.Enums;
using Todo.Model;
using Todo.Model.FilterDto;
using Todo.Storage.Context;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Storage.Repository.Providers;

public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _context;
    public CommentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddCommentAsync(CommentEntity comment)
    {
        await _context.Comments.AddAsync(comment);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteCommentByIdAsync(CommentEntity comment)
    {
        _context.Comments.Remove(comment);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<CommentEntity?> GetCommentById(string id)
    {
        return await _context.Comments.FindAsync(id);
    }

    public async Task<bool> UpdateCommentAsync(CommentEntity comment)
    {
        _context.Comments.Update(comment);
        return await _context.SaveChangesAsync() > 0;
    }

    // Implement pagination method using FilterDto approach
    // This method implements filtering, sorting, and pagination
    public async Task<PageResultResponseDto<CommentEntity>> GetCommentsWithPaginationAsync(CommentFilterDto commentFilter)
    {
        var query = _context.Comments.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(commentFilter.UserId))
        {
            query = query.Where(c => c.UserId == commentFilter.UserId);
        }

        if (!string.IsNullOrWhiteSpace(commentFilter.ActivityId))
        {
            query = query.Where(c => c.ActivityId == commentFilter.ActivityId);
        }

        if (!string.IsNullOrWhiteSpace(commentFilter.Message))
        {
            query = query.Where(c => c.Message.ToLower().Contains(commentFilter.Message.ToLower()));
        }

        // Get total count
        var count = await query.CountAsync();

        // Create page result
        var pageResult = new PageResultResponseDto<CommentEntity>
        {
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)commentFilter.PageSize),
            Page = commentFilter.Page,
            PageSize = commentFilter.PageSize
        };

        var offset = (commentFilter.Page - 1) * commentFilter.PageSize;

        // Apply sorting and pagination
        if (commentFilter.Sort == SortDirection.Desc)
        {
            pageResult.Records = await query.OrderByDescending(f => f.CreatedOn).Skip(offset)
                .Take(commentFilter.PageSize).ToListAsync();
        }
        
        pageResult.Records = await query.OrderBy(f => f.CreatedOn).Skip(offset).Take(commentFilter.PageSize).ToListAsync();

        return pageResult;
    }
}

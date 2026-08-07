using Microsoft.EntityFrameworkCore;
using Todo.Entities;
using Todo.Enums;
using Todo.Model;
using Todo.Model.FilterDto;
using Todo.Storage.Context;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Storage.Repository.Providers;

public class ActivityRepository : IActivityRepository
{
    private readonly ApplicationDbContext _context;
    public ActivityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddActivityAsync(ActivityEntity activity)
    {
        await _context.Activities.AddAsync(activity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteActivityByIdAsync(ActivityEntity activity)
    {
        _context.Activities.Remove(activity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<ActivityEntity?> GetActivityById(string id)
    {
        return await _context.Activities.FindAsync(id);
    }

    public async Task<bool> UpdateActivityAsync(ActivityEntity activity)
    {
        _context.Activities.Update(activity);
        return await _context.SaveChangesAsync() > 0;
    }

    // Implement pagination method using FilterDto approach
    // This method implements filtering, sorting, and pagination
    public async Task<PageResultResponseDto<ActivityEntity>> GetActivitiesWithPaginationAsync(ActivityFilterDto activityFilter)
    {
        var query = _context.Activities.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(activityFilter.UserId))
        {
            query = query.Where(a => a.UserId == activityFilter.UserId);
        }

        if (!string.IsNullOrWhiteSpace(activityFilter.CategoryId))
        {
            query = query.Where(a => a.CategoryId == activityFilter.CategoryId);
        }

        if (!string.IsNullOrWhiteSpace(activityFilter.Title))
        {
            query = query.Where(a => a.Title.ToLower().Contains(activityFilter.Title.ToLower()));
        }

        if (activityFilter.Status.HasValue)
        {
            query = query.Where(a => a.Status == activityFilter.Status.Value);
        }

        if (activityFilter.Priority.HasValue)
        {
            query = query.Where(a => a.Priority == activityFilter.Priority.Value);
        }

        if (activityFilter.StartedOn.HasValue)
        {
            query = query.Where(a => a.StartedOn.HasValue && a.StartedOn.Value.Date == activityFilter.StartedOn.Value.Date);
        }

        if (activityFilter.EndedOn.HasValue)
        {
            query = query.Where(a => a.EndedOn.HasValue && a.EndedOn.Value.Date == activityFilter.EndedOn.Value.Date);
        }

        // Get total count
        var count = await query.CountAsync();

        // Create page result
        var pageResult = new PageResultResponseDto<ActivityEntity>
        {
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)activityFilter.PageSize),
            Page = activityFilter.Page,
            PageSize = activityFilter.PageSize
        };

        var offset = (activityFilter.Page - 1) * activityFilter.PageSize;

        // Apply sorting and pagination
        if (activityFilter.Sort == SortDirection.Desc)
        {
            pageResult.Records = await query.OrderByDescending(f => f.CreatedOn).Skip(offset)
                .Take(activityFilter.PageSize).ToListAsync();
        }
       
       pageResult.Records = await query.OrderBy(f => f.CreatedOn).Skip(offset).Take(activityFilter.PageSize).ToListAsync();

        return pageResult;
    }
}

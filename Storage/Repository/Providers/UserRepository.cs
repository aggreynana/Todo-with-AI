using Microsoft.EntityFrameworkCore;
using Todo.Entities;
using Todo.Enums;
using Todo.Model;
using Todo.Model.FilterDto;
using Todo.Storage.Context;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Storage.Repository.Providers;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> AddUserAsync(UserEntity user)
    {
        await _context.Users.AddAsync(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteUserByIdAsync(UserEntity user)
    {
        _context.Users.Remove(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<UserEntity?> GetUserById(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    // STEP 5: Implement method to get user by email
    // This method is used for authentication where email is the login identifier
    public async Task<UserEntity?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> UpdateUSerAsync(UserEntity user)
    {
        _context.Users.Update(user);
        return await _context.SaveChangesAsync() > 0;
    }

    // STEP 6: Implement pagination method using FilterDto approach
    // This method implements filtering, sorting, and pagination
    public async Task<PageResultResponseDto<UserEntity>> GetUsersWithPaginationAsync(UserFilterDto userFilter)
    {
        var query = _context.Users.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(userFilter.FirstName))
        {
            query = query.Where(u => u.FirstName.ToLower().Contains(userFilter.FirstName.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(userFilter.LastName))
        {
            query = query.Where(u => u.LastName.ToLower().Contains(userFilter.LastName.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(userFilter.Email))
        {
            query = query.Where(u => u.Email.ToLower().Contains(userFilter.Email.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(userFilter.MiddleName))
        {
            query = query.Where(u => u.MiddleName != null && u.MiddleName.ToLower().Contains(userFilter.MiddleName.ToLower()));
        }

        if (userFilter.CreatedOn.HasValue)
        {
            query = query.Where(u => u.CreatedOn.Date == userFilter.CreatedOn.Value.Date);
        }

        // Get total count
        var count = await query.CountAsync();

        // Create page result
        var pageResult = new PageResultResponseDto<UserEntity>
        {
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)userFilter.PageSize),
            Page = userFilter.Page,
            PageSize = userFilter.PageSize
        };

        var offset = (userFilter.Page - 1) * userFilter.PageSize;

        // Apply sorting and pagination
        if (userFilter.Sort == SortDirection.Desc)
        {
            pageResult.Records = await query.OrderByDescending(f => f.CreatedOn).Skip(offset)
                .Take(userFilter.PageSize).ToListAsync();
        }
        
        pageResult.Records = await query.OrderBy(f => f.CreatedOn).Skip(offset).Take(userFilter.PageSize).ToListAsync();

        return pageResult;
    }
}

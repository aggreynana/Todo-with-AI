using Microsoft.EntityFrameworkCore;
using Todo.Entities;
using Todo.Enums;
using Todo.Model;
using Todo.Model.FilterDto;
using Todo.Storage.Context;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Storage.Repository.Providers;

public class FileAttachmentRepository : IFileAttachmentRepository
{
    private readonly ApplicationDbContext _context;
    public FileAttachmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddFileAttachmentAsync(FileAttachmentEntity fileAttachment)
    {
        await _context.FileAttachments.AddAsync(fileAttachment);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteFileAttachmentByIdAsync(FileAttachmentEntity fileAttachment)
    {
        _context.FileAttachments.Remove(fileAttachment);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<FileAttachmentEntity?> GetFileAttachmentById(string id)
    {
        return await _context.FileAttachments.FindAsync(id);
    }

    public async Task<bool> UpdateFileAttachmentAsync(FileAttachmentEntity fileAttachment)
    {
        _context.FileAttachments.Update(fileAttachment);
        return await _context.SaveChangesAsync() > 0;
    }

    // Implement pagination method using FilterDto approach
    // This method implements filtering, sorting, and pagination
    public async Task<PageResultResponseDto<FileAttachmentEntity>> GetFileAttachmentsWithPaginationAsync(FileAttachmentFilterDto fileAttachmentFilter)
    {
        var query = _context.FileAttachments.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(fileAttachmentFilter.UserId))
        {
            query = query.Where(f => f.UserId == fileAttachmentFilter.UserId);
        }

        if (!string.IsNullOrWhiteSpace(fileAttachmentFilter.ActivityId))
        {
            query = query.Where(f => f.ActivityId == fileAttachmentFilter.ActivityId);
        }

        if (!string.IsNullOrWhiteSpace(fileAttachmentFilter.FileName))
        {
            query = query.Where(f => f.FileName.ToLower().Contains(fileAttachmentFilter.FileName.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(fileAttachmentFilter.ContentType))
        {
            query = query.Where(f => f.ContentType.ToLower().Contains(fileAttachmentFilter.ContentType.ToLower()));
        }

        if (fileAttachmentFilter.UpLoadedOn.HasValue)
        {
            query = query.Where(f => f.UpLoadedOn.HasValue && f.UpLoadedOn.Value.Date == fileAttachmentFilter.UpLoadedOn.Value.Date);
        }

        // Get total count
        var count = await query.CountAsync();

        // Create page result
        var pageResult = new PageResultResponseDto<FileAttachmentEntity>
        {
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)fileAttachmentFilter.PageSize),
            Page = fileAttachmentFilter.Page,
            PageSize = fileAttachmentFilter.PageSize
        };

        var offset = (fileAttachmentFilter.Page - 1) * fileAttachmentFilter.PageSize;

        // Apply sorting and pagination
        if (fileAttachmentFilter.Sort == SortDirection.Desc)
        {
            pageResult.Records = await query.OrderByDescending(f => f.CreatedOn).Skip(offset)
                .Take(fileAttachmentFilter.PageSize).ToListAsync();
        }
        
        pageResult.Records = await query.OrderBy(f => f.CreatedOn).Skip(offset).Take(fileAttachmentFilter.PageSize).ToListAsync();

        return pageResult;
    }
}

using Todo.Entities;
using Todo.Model;
using Todo.Model.FilterDto;

namespace Todo.Storage.Repository.Interfaces;

public interface IFileAttachmentRepository
{
    Task<bool> AddFileAttachmentAsync(FileAttachmentEntity fileAttachment);
    Task<FileAttachmentEntity?> GetFileAttachmentById(string id);
    Task<bool> UpdateFileAttachmentAsync(FileAttachmentEntity fileAttachment);
    Task<bool> DeleteFileAttachmentByIdAsync(FileAttachmentEntity fileAttachment);

    // Pagination method using FilterDto approach
    // This method supports filtering, sorting, and pagination
    // Returns PageResultResponseDto with all pagination metadata
    Task<PageResultResponseDto<FileAttachmentEntity>> GetFileAttachmentsWithPaginationAsync(FileAttachmentFilterDto fileAttachmentFilter);
}

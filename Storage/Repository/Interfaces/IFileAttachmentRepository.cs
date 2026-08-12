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

    Task<PageResultResponseDto<FileAttachmentEntity>> GetFileAttachmentsWithPaginationAsync(FileAttachmentFilterDto fileAttachmentFilter);
}

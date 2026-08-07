using Todo.Model;
using Todo.Model.FileAttachmentDto;
using Todo.Model.FilterDto;

namespace Todo.Services.Interfaces;

// Define the interface for FileAttachment service
// This interface defines the contract for all business logic operations related to FileAttachment entities
public interface IFileAttachmentService
{
    // Create method for adding a new file attachment
    // Takes a DTO as input to create a new file attachment
    // Returns a wrapped response with the created file attachment DTO
    Task<ApiResponse<GetFileAttachmentResponseDto>> CreateFileAttachmentAsync(CreateFileAttachmentRequestDto fileAttachmentDto);

    // Create method to retrieve a single file attachment by its ID
    // Returns nullable response since the file attachment might not exist
    Task<ApiResponse<GetFileAttachmentResponseDto>?> GetFileAttachmentByIdAsync(string id);

    // Create method to retrieve all file attachments with pagination and filtering
    // Returns a wrapped response with paginated file attachment DTOs
    Task<ApiResponse<PageResultResponseDto<GetFileAttachmentResponseDto>>> GetFileAttachmentsAsync(FileAttachmentFilterDto? fileAttachmentFilter = null);

    // Create method to update an existing file attachment
    // Takes an update DTO and returns the updated file attachment DTO
    Task<ApiResponse<GetFileAttachmentResponseDto>> UpdateFileAttachmentAsync(string id, UpdateFileAttachmentRequestDto fileAttachmentUpdate);

    // Create method to delete a file attachment by its ID
    // Returns a boolean indicating success/failure
    Task<ApiResponse<bool>> DeleteFileAttachmentByIdAsync(string id);
}

using System.Linq;
using System;
using Todo.Entities;
using Todo.Model;
using Todo.Model.FileAttachmentDto;
using Todo.Model.FilterDto;
using Todo.Services.Interfaces;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Services.Providers;

// STEP 1: Implement the IFileAttachmentService interface
// This class contains the business logic for FileAttachment operations
// It acts as a bridge between the controller and the repository layer
public class FileAttachmentService : IFileAttachmentService
{
    // STEP 2: Inject the IFileAttachmentRepository through constructor injection
    // This follows the dependency injection pattern for loose coupling
    private readonly IFileAttachmentRepository _fileAttachmentRepository;

    public FileAttachmentService(IFileAttachmentRepository fileAttachmentRepository)
    {
        _fileAttachmentRepository = fileAttachmentRepository;
    }

    // STEP 3: Implement CreateFileAttachmentAsync method
    // This method handles the creation of a new file attachment
    public async Task<ApiResponse<GetFileAttachmentResponseDto>> CreateFileAttachmentAsync(CreateFileAttachmentRequestDto fileAttachmentDto)
    {
        // STEP 4: Map the DTO to the entity
        // Convert the incoming DTO to the domain entity for database operations
        var fileAttachmentEntity = new FileAttachmentEntity
        {
            UserId = fileAttachmentDto.UserId,
            ActivityId = fileAttachmentDto.ActivityId,
            FileName = fileAttachmentDto.FileName,
            FilePath = fileAttachmentDto.FilePath,
            ContentType = fileAttachmentDto.ContentType,
            UpLoadedOn = fileAttachmentDto.UpLoadedOn ?? DateTime.UtcNow
            // Id and CreatedOn are set automatically in BaseEntity
        };

        // STEP 5: Call the repository to add the entity to the database
        var result = await _fileAttachmentRepository.AddFileAttachmentAsync(fileAttachmentEntity);

        // STEP 6: Check if the operation was successful
        if (!result)
        {
            // STEP 7: Return an error response if creation failed
            return ApiResponse<GetFileAttachmentResponseDto>.InternalServerError();
        }

        // STEP 8: Map the entity back to DTO for the response
        var responseDto = MapToResponseDto(fileAttachmentEntity);

        // STEP 9: Return a success response with the created file attachment
        return ApiResponse<GetFileAttachmentResponseDto>.CreatedResponse("FileAttachment", responseDto);
    }

    // STEP 10: Implement GetFileAttachmentByIdAsync method
    // This method retrieves a single file attachment by its unique identifier
    public async Task<ApiResponse<GetFileAttachmentResponseDto>?> GetFileAttachmentByIdAsync(string id)
    {
        // STEP 11: Call the repository to get the file attachment
        var fileAttachmentEntity = await _fileAttachmentRepository.GetFileAttachmentById(id);

        // STEP 12: Check if the file attachment exists
        if (fileAttachmentEntity == null)
        {
            // STEP 13: Return null if file attachment not found
            return null;
        }

        // STEP 14: Map the entity to DTO
        var responseDto = MapToResponseDto(fileAttachmentEntity);

        // STEP 15: Return the file attachment in a success response
        return ApiResponse<GetFileAttachmentResponseDto>.OkResponse("File attachment retrieved successfully", responseDto);
    }

    // Implement GetFileAttachmentsAsync method with pagination and filtering
    // This method retrieves file attachments from the database with pagination and filtering support
    public async Task<ApiResponse<PageResultResponseDto<GetFileAttachmentResponseDto>>> GetFileAttachmentsAsync(FileAttachmentFilterDto? fileAttachmentFilter = null)
    {
        // Use default filter if not provided
        var filter = fileAttachmentFilter ?? new FileAttachmentFilterDto();

        // Call the repository with filter
        var fileAttachmentPageResult = await _fileAttachmentRepository.GetFileAttachmentsWithPaginationAsync(filter);

        // Map entities to DTOs
        var fileAttachmentDtos = fileAttachmentPageResult.Records.Select(MapToResponseDto).ToList();

        // Create paginated response with metadata
        var pageResult = new PageResultResponseDto<GetFileAttachmentResponseDto>
        {
            Page = fileAttachmentPageResult.Page,
            PageSize = fileAttachmentPageResult.PageSize,
            Records = fileAttachmentDtos,
            TotalCount = fileAttachmentPageResult.TotalCount,
            TotalPages = fileAttachmentPageResult.TotalPages
        };

        // Return paginated response
        return ApiResponse<PageResultResponseDto<GetFileAttachmentResponseDto>>.OkResponse("File attachments retrieved successfully", pageResult);
    }

    // STEP 28: Implement UpdateFileAttachmentAsync method
    // This method updates an existing file attachment
    public async Task<ApiResponse<GetFileAttachmentResponseDto>> UpdateFileAttachmentAsync(string id, UpdateFileAttachmentRequestDto fileAttachmentUpdate)
    {
        // STEP 29: First, retrieve the existing file attachment
        var existingFileAttachment = await _fileAttachmentRepository.GetFileAttachmentById(id);

        // STEP 30: Check if the file attachment exists
        if (existingFileAttachment == null)
        {
            // STEP 31: Return an error response if file attachment not found
            return ApiResponse<GetFileAttachmentResponseDto>.NotFound("File attachment not found");
        }

        // STEP 32: Update only the fields that are provided (partial update)
        if (fileAttachmentUpdate.FileName != null)
            existingFileAttachment.FileName = fileAttachmentUpdate.FileName;
        if (fileAttachmentUpdate.FilePath != null)
            existingFileAttachment.FilePath = fileAttachmentUpdate.FilePath;
        if (fileAttachmentUpdate.ContentType != null)
            existingFileAttachment.ContentType = fileAttachmentUpdate.ContentType;
        if (fileAttachmentUpdate.UpLoadedOn.HasValue)
            existingFileAttachment.UpLoadedOn = fileAttachmentUpdate.UpLoadedOn;

        // STEP 33: Set the ModifiedOn timestamp
        existingFileAttachment.ModifiedOn = DateTime.UtcNow;

        // STEP 34: Call the repository to update the entity
        var result = await _fileAttachmentRepository.UpdateFileAttachmentAsync(existingFileAttachment);

        // STEP 35: Check if the update was successful
        if (!result)
        {
            // STEP 36: Return an error response if update failed
            return ApiResponse<GetFileAttachmentResponseDto>.InternalServerError();
        }

        // STEP 37: Map the updated entity to DTO
        var responseDto = MapToResponseDto(existingFileAttachment);

        // STEP 38: Return a success response with the updated file attachment
        return ApiResponse<GetFileAttachmentResponseDto>.AcceptedResponse();
    }

    // STEP 39: Implement DeleteFileAttachmentByIdAsync method
    // This method deletes a file attachment by its ID
    public async Task<ApiResponse<bool>> DeleteFileAttachmentByIdAsync(string id)
    {
        // STEP 40: First, retrieve the existing file attachment
        var existingFileAttachment = await _fileAttachmentRepository.GetFileAttachmentById(id);

        // STEP 41: Check if the file attachment exists
        if (existingFileAttachment == null)
        {
            // STEP 42: Return an error response if file attachment not found
            return ApiResponse<bool>.NotFound("File attachment not found");
        }

        // STEP 43: Call the repository to delete the entity
        var result = await _fileAttachmentRepository.DeleteFileAttachmentByIdAsync(existingFileAttachment);

        // STEP 44: Check if the deletion was successful
        if (!result)
        {
            // STEP 45: Return an error response if deletion failed
            return ApiResponse<bool>.InternalServerError();
        }

        // STEP 46: Return a success response indicating successful deletion
        return ApiResponse<bool>.NoContent();
    }

    // STEP 47: Create a helper method to map Entity to DTO
    // This private method reduces code duplication and ensures consistent mapping
    private static GetFileAttachmentResponseDto MapToResponseDto(FileAttachmentEntity entity)
    {
        return new GetFileAttachmentResponseDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            ActivityId = entity.ActivityId,
            FileName = entity.FileName,
            FilePath = entity.FilePath,
            ContentType = entity.ContentType,
            UpLoadedOn = entity.UpLoadedOn,
            CreatedOn = entity.CreatedOn,
            ModifiedOn = entity.ModifiedOn
        };
    }
}

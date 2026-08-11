using System.Linq;
using System;
using Todo.Entities;
using Todo.Model;
using Todo.Model.FileAttachmentDto;
using Todo.Model.FilterDto;
using Todo.Services.Interfaces;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Services.Providers;

public class FileAttachmentService : IFileAttachmentService
{
    private readonly IFileAttachmentRepository _fileAttachmentRepository;
    private readonly ICacheService _cacheService;

    public FileAttachmentService(IFileAttachmentRepository fileAttachmentRepository, ICacheService cacheService)
    {
        _fileAttachmentRepository = fileAttachmentRepository;
        _cacheService = cacheService;
    }

    // STEP 3: Implement CreateFileAttachmentAsync method
    // This method handles the creation of a new file attachment
    public async Task<ApiResponse<GetFileAttachmentResponseDto>> CreateFileAttachmentAsync(CreateFileAttachmentRequestDto fileAttachmentDto)
    {
        var file = await _fileAttachmentRepository.GetFileAttachmentById(fileAttachmentDto.FileName);

        if (file != null) return ApiResponse<GetFileAttachmentResponseDto>.FailedDependency();

        var fileAttachmentEntity = new FileAttachmentEntity
        {
            UserId = fileAttachmentDto.UserId,
            ActivityId = fileAttachmentDto.ActivityId,
            FileName = fileAttachmentDto.FileName,
            FilePath = fileAttachmentDto.FilePath,
            ContentType = fileAttachmentDto.ContentType,
            UpLoadedOn = fileAttachmentDto.UpLoadedOn ?? DateTime.UtcNow
        };

        var result = await _fileAttachmentRepository.AddFileAttachmentAsync(fileAttachmentEntity);

        if (!result)
        {
            return ApiResponse<GetFileAttachmentResponseDto>.InternalServerError();
        }

        // Map the entity back to DTO for the response
        var responseDto = MapToResponseDto(fileAttachmentEntity);

        return ApiResponse<GetFileAttachmentResponseDto>.CreatedResponse("FileAttachment", responseDto);
    }


    public async Task<ApiResponse<GetFileAttachmentResponseDto>?> GetFileAttachmentByIdAsync(string id)
    {
        // Try to get from cache first
        var cacheKey = $"fileattachment_{id}";
        var cachedFileAttachment = await _cacheService.GetAsync<GetFileAttachmentResponseDto>(cacheKey);

        if (cachedFileAttachment != null)
        {
            return ApiResponse<GetFileAttachmentResponseDto>.OkResponse("File attachment retrieved from cache", cachedFileAttachment);
        }

        var fileAttachmentEntity = await _fileAttachmentRepository.GetFileAttachmentById(id);


        if (fileAttachmentEntity == null)
        {
            return ApiResponse<GetFileAttachmentResponseDto>.InternalServerError();
        }

        //  Map the entity to DTO
        var responseDto = MapToResponseDto(fileAttachmentEntity);

        // Cache the result for 5 minutes
        await _cacheService.SetAsync(cacheKey, responseDto, TimeSpan.FromMinutes(5));

        // Return the file attachment in a success response
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
            
        if (fileAttachmentUpdate.ContentType != null)
            existingFileAttachment.ContentType = fileAttachmentUpdate.ContentType;
            
        existingFileAttachment.FilePath = fileAttachmentUpdate.FilePath;
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

        // Invalidate cache for this file attachment
        var cacheKey = $"fileattachment_{id}";
        await _cacheService.RemoveAsync(cacheKey);

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

        // Invalidate cache for this file attachment
        var cacheKey = $"fileattachment_{id}";
        await _cacheService.RemoveAsync(cacheKey);

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

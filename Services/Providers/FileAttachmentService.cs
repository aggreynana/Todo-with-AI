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
    private readonly ILogger<FileAttachmentService> _logger;

    public FileAttachmentService(IFileAttachmentRepository fileAttachmentRepository, ICacheService cacheService, ILogger<FileAttachmentService> logger)
    {
        _fileAttachmentRepository = fileAttachmentRepository;
        _cacheService = cacheService;
        _logger = logger;
    }


    public async Task<ApiResponse<GetFileAttachmentResponseDto>> CreateFileAttachmentAsync(CreateFileAttachmentRequestDto fileAttachmentDto)
    {
        _logger.LogInformation("Creating file attachment: {FileName} for activity: {ActivityId}", fileAttachmentDto.FileName, fileAttachmentDto.ActivityId);
        var file = await _fileAttachmentRepository.GetFileAttachmentById(fileAttachmentDto.FileName);

        if (file != null)
        {
            _logger.LogWarning("File attachment creation failed - file already exists: {FileName}", fileAttachmentDto.FileName);
            return ApiResponse<GetFileAttachmentResponseDto>.FailedDependency();
        }

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
            _logger.LogError("File attachment creation failed for file: {FileName}", fileAttachmentDto.FileName);
            return ApiResponse<GetFileAttachmentResponseDto>.InternalServerError();
        }

        // Map the entity back to DTO for the response
        var responseDto = MapToResponseDto(fileAttachmentEntity);
        _logger.LogInformation("File attachment created successfully with ID: {FileAttachmentId}", fileAttachmentEntity.Id);

        return ApiResponse<GetFileAttachmentResponseDto>.CreatedResponse("FileAttachment", responseDto);
    }


    public async Task<ApiResponse<GetFileAttachmentResponseDto>?> GetFileAttachmentByIdAsync(string id)
    {
        _logger.LogInformation("Fetching file attachment with ID: {FileAttachmentId}", id);
        // Try to get from cache first
        var cacheKey = $"fileattachment_{id}";
        var cachedFileAttachment = await _cacheService.GetAsync<GetFileAttachmentResponseDto>(cacheKey);

        if (cachedFileAttachment != null)
        {
            _logger.LogInformation("File attachment retrieved from cache with ID: {FileAttachmentId}", id);
            return ApiResponse<GetFileAttachmentResponseDto>.OkResponse("File attachment retrieved from cache", cachedFileAttachment);
        }

        var fileAttachmentEntity = await _fileAttachmentRepository.GetFileAttachmentById(id);

        if (fileAttachmentEntity == null)
        {
            _logger.LogWarning("File attachment not found with ID: {FileAttachmentId}", id);
            return ApiResponse<GetFileAttachmentResponseDto>.InternalServerError();
        }

        //  Map the entity to DTO
        var responseDto = MapToResponseDto(fileAttachmentEntity);

        // Cache the result for 5 minutes
        await _cacheService.SetAsync(cacheKey, responseDto, TimeSpan.FromMinutes(5));

        _logger.LogInformation("File attachment retrieved successfully with ID: {FileAttachmentId}", id);

        return ApiResponse<GetFileAttachmentResponseDto>.OkResponse("File attachment retrieved successfully", responseDto);
    }


    public async Task<ApiResponse<PageResultResponseDto<GetFileAttachmentResponseDto>>> GetFileAttachmentsAsync(FileAttachmentFilterDto? fileAttachmentFilter = null)
    {
        _logger.LogInformation("Fetching file attachments with filter: {@Filter}", fileAttachmentFilter);
        // Use default filter if not provided
        var filter = fileAttachmentFilter ?? new FileAttachmentFilterDto();


        var fileAttachmentPageResult = await _fileAttachmentRepository.GetFileAttachmentsWithPaginationAsync(filter);

        // Map entities to DTOs
        var fileAttachmentDtos = fileAttachmentPageResult.Records.Select(MapToResponseDto).ToList();

        var pageResult = new PageResultResponseDto<GetFileAttachmentResponseDto>
        {
            Page = fileAttachmentPageResult.Page,
            PageSize = fileAttachmentPageResult.PageSize,
            Records = fileAttachmentDtos,
            TotalCount = fileAttachmentPageResult.TotalCount,
            TotalPages = fileAttachmentPageResult.TotalPages
        };

        _logger.LogInformation("Retrieved {Count} file attachments", fileAttachmentDtos.Count);

        return ApiResponse<PageResultResponseDto<GetFileAttachmentResponseDto>>.OkResponse("File attachments retrieved successfully", pageResult);
    }


    public async Task<ApiResponse<GetFileAttachmentResponseDto>> UpdateFileAttachmentAsync(string id, UpdateFileAttachmentRequestDto fileAttachmentUpdate)
    {
        _logger.LogInformation("Updating file attachment with ID: {FileAttachmentId}", id);
        var existingFileAttachment = await _fileAttachmentRepository.GetFileAttachmentById(id);

        if (existingFileAttachment == null)
        {
            _logger.LogWarning("File attachment not found for update with ID: {FileAttachmentId}", id);
            return ApiResponse<GetFileAttachmentResponseDto>.NotFound("File attachment not found");
        }

        // Update only the fields that are provided (partial update)
        if (fileAttachmentUpdate.FileName != null)
            existingFileAttachment.FileName = fileAttachmentUpdate.FileName;

        if (fileAttachmentUpdate.ContentType != null)
            existingFileAttachment.ContentType = fileAttachmentUpdate.ContentType;

        existingFileAttachment.FilePath = fileAttachmentUpdate.FilePath;
        existingFileAttachment.UpLoadedOn = fileAttachmentUpdate.UpLoadedOn;

        existingFileAttachment.ModifiedOn = DateTime.UtcNow;

        var result = await _fileAttachmentRepository.UpdateFileAttachmentAsync(existingFileAttachment);

        if (!result)
        {
            _logger.LogError("File attachment update failed for ID: {FileAttachmentId}", id);
            return ApiResponse<GetFileAttachmentResponseDto>.InternalServerError();
        }

        // Map the updated entity to DTO
        var responseDto = MapToResponseDto(existingFileAttachment);

        // Invalidate cache for this file attachment
        var cacheKey = $"fileattachment_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        _logger.LogInformation("File attachment updated successfully with ID: {FileAttachmentId}", id);
        return ApiResponse<GetFileAttachmentResponseDto>.AcceptedResponse();
    }


    public async Task<ApiResponse<bool>> DeleteFileAttachmentByIdAsync(string id)
    {
        _logger.LogInformation("Deleting file attachment with ID: {FileAttachmentId}", id);
        var existingFileAttachment = await _fileAttachmentRepository.GetFileAttachmentById(id);

        if (existingFileAttachment == null)
        {
            _logger.LogWarning("File attachment not found for deletion with ID: {FileAttachmentId}", id);
            return ApiResponse<bool>.NotFound("File attachment not found");
        }

        var result = await _fileAttachmentRepository.DeleteFileAttachmentByIdAsync(existingFileAttachment);

        if (!result)
        {
            _logger.LogError("File attachment deletion failed for ID: {FileAttachmentId}", id);
            return ApiResponse<bool>.InternalServerError();
        }

        // Invalidate cache for this file attachment
        var cacheKey = $"fileattachment_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        _logger.LogInformation("File attachment deleted successfully with ID: {FileAttachmentId}", id);
        return ApiResponse<bool>.NoContent();
    }

    // Create a helper method to map Entity to DTO
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

using Todo.Entities;
using Todo.Model;
using Todo.Model.CommentDto;
using Todo.Model.FilterDto;
using Todo.Services.Interfaces;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Services.Providers;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CommentService> _logger;

    public CommentService(ICommentRepository commentRepository, ICacheService cacheService, ILogger<CommentService> logger)
    {
        _commentRepository = commentRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse<GetCommentResponseDto>> CreateCommentAsync(CreateCommentRequestDto commentDto)
    {
        _logger.LogInformation("Creating comment for activity: {ActivityId} by user: {UserId}", commentDto.ActivityId, commentDto.UserId);
        var commentEntity = new CommentEntity
        {
            UserId = commentDto.UserId,
            ActivityId = commentDto.ActivityId,
            Message = commentDto.Message
        };

        var result = await _commentRepository.AddCommentAsync(commentEntity);

        if (!result)
        {
            _logger.LogError("Comment creation failed for activity: {ActivityId}", commentDto.ActivityId);
            return ApiResponse<GetCommentResponseDto>.InternalServerError();
        }

        var responseDto = MapToResponseDto(commentEntity);
        _logger.LogInformation("Comment created successfully with ID: {CommentId}", commentEntity.Id);

        return ApiResponse<GetCommentResponseDto>.CreatedResponse("Comment", responseDto);
    }


    public async Task<ApiResponse<GetCommentResponseDto>?> GetCommentByIdAsync(string id)
    {
        _logger.LogInformation("Fetching comment with ID: {CommentId}", id);
        // Try to get from cache first
        var cacheKey = $"comment_{id}";
        var cachedComment = await _cacheService.GetAsync<GetCommentResponseDto>(cacheKey);

        if (cachedComment != null)
        {
            _logger.LogInformation("Comment retrieved from cache with ID: {CommentId}", id);
            return ApiResponse<GetCommentResponseDto>.OkResponse("Comment retrieved from cache", cachedComment);
        }

        var commentEntity = await _commentRepository.GetCommentById(id);

        if (commentEntity == null)
        {
            _logger.LogWarning("Comment not found with ID: {CommentId}", id);
            return null;
        }

        var responseDto = MapToResponseDto(commentEntity);

        // Cache the result for 5 minutes
        await _cacheService.SetAsync(cacheKey, responseDto, TimeSpan.FromMinutes(5));

        _logger.LogInformation("Comment retrieved successfully with ID: {CommentId}", id);
        return ApiResponse<GetCommentResponseDto>.OkResponse("Comment retrieved successfully", responseDto);
    }


    public async Task<ApiResponse<PageResultResponseDto<GetCommentResponseDto>>> GetCommentsAsync(CommentFilterDto? commentFilter = null)
    {
        _logger.LogInformation("Fetching comments with filter: {@Filter}", commentFilter);
        var filter = commentFilter ?? new CommentFilterDto();

        var commentPageResult = await _commentRepository.GetCommentsWithPaginationAsync(filter);

        var commentDtos = commentPageResult.Records.Select(MapToResponseDto).ToList();

        // Create paginated response with metadata
        var pageResult = new PageResultResponseDto<GetCommentResponseDto>
        {
            Page = commentPageResult.Page,
            PageSize = commentPageResult.PageSize,
            Records = commentDtos,
            TotalCount = commentPageResult.TotalCount,
            TotalPages = commentPageResult.TotalPages
        };

        _logger.LogInformation("Retrieved {Count} comments", commentDtos.Count);

        return ApiResponse<PageResultResponseDto<GetCommentResponseDto>>.OkResponse("Comments retrieved successfully", pageResult);
    }


    public async Task<ApiResponse<GetCommentResponseDto>> UpdateCommentAsync(string id, UpdateCommentRequestDto commentUpdate)
    {
        _logger.LogInformation("Updating comment with ID: {CommentId}", id);
        var existingComment = await _commentRepository.GetCommentById(id);

        if (existingComment == null)
        {
            _logger.LogWarning("Comment not found for update with ID: {CommentId}", id);
            return ApiResponse<GetCommentResponseDto>.NotFound("Comment not found");
        }

        // STEP 32: Update only the fields that are provided (partial update)
        if (commentUpdate.Message != null) 
            existingComment.Message = commentUpdate.Message;

        existingComment.ModifiedOn = DateTime.UtcNow;


        var result = await _commentRepository.UpdateCommentAsync(existingComment);

        if (!result)
        {
            _logger.LogError("Comment update failed for ID: {CommentId}", id);

            return ApiResponse<GetCommentResponseDto>.InternalServerError();
        }

        // Map the updated entity to DTO
        var responseDto = MapToResponseDto(existingComment);

        // Invalidate cache for this comment
        var cacheKey = $"comment_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        _logger.LogInformation("Comment updated successfully with ID: {CommentId}", id);
        return ApiResponse<GetCommentResponseDto>.AcceptedResponse();
    }

    // Implement DeleteCommentByIdAsync method
    // This method deletes a comment by its ID
    public async Task<ApiResponse<bool>> DeleteCommentByIdAsync(string id)
    {
        _logger.LogInformation("Deleting comment with ID: {CommentId}", id);
        // STEP 40: First, retrieve the existing comment
        var existingComment = await _commentRepository.GetCommentById(id);

        // Check if the comment exists
        if (existingComment == null)
        {
            _logger.LogWarning("Comment not found for deletion with ID: {CommentId}", id);

            return ApiResponse<bool>.NotFound("Comment not found");
        }

        var result = await _commentRepository.DeleteCommentByIdAsync(existingComment);

        // Check if the deletion was successful
        if (!result)
        {
            _logger.LogError("Comment deletion failed for ID: {CommentId}", id);
            return ApiResponse<bool>.InternalServerError();
        }

        // Invalidate cache for this comment
        var cacheKey = $"comment_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        _logger.LogInformation("Comment deleted successfully with ID: {CommentId}", id);
        return ApiResponse<bool>.NoContent();
    }

    // Create a helper method to map Entity to DTO
    // This private method reduces code duplication and ensures consistent mapping
    private static GetCommentResponseDto MapToResponseDto(CommentEntity entity)
    {
        return new GetCommentResponseDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            ActivityId = entity.ActivityId,
            Message = entity.Message,
            CreatedOn = entity.CreatedOn,
            ModifiedOn = entity.ModifiedOn
        };
    }
}

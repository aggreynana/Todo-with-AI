using System.Linq;
using System;
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

    public CommentService(ICommentRepository commentRepository, ICacheService cacheService)
    {
        _commentRepository = commentRepository;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<GetCommentResponseDto>> CreateCommentAsync(CreateCommentRequestDto commentDto)
    {
        var commentEntity = new CommentEntity
        {
            UserId = commentDto.UserId,
            ActivityId = commentDto.ActivityId,
            Message = commentDto.Message
        };

        var result = await _commentRepository.AddCommentAsync(commentEntity);

        if (!result)
        {
            return ApiResponse<GetCommentResponseDto>.InternalServerError();
        }

        var responseDto = MapToResponseDto(commentEntity);

        return ApiResponse<GetCommentResponseDto>.CreatedResponse("Comment", responseDto);
    }


    public async Task<ApiResponse<GetCommentResponseDto>?> GetCommentByIdAsync(string id)
    {
        // Try to get from cache first
        var cacheKey = $"comment_{id}";
        var cachedComment = await _cacheService.GetAsync<GetCommentResponseDto>(cacheKey);

        if (cachedComment != null)
        {
            return ApiResponse<GetCommentResponseDto>.OkResponse("Comment retrieved from cache", cachedComment);
        }

        // STEP 11: Call the repository to get the comment
        var commentEntity = await _commentRepository.GetCommentById(id);

        // STEP 12: Check if the comment exists
        if (commentEntity == null)
        {
            // STEP 13: Return null if comment not found
            return null;
        }

        // STEP 14: Map the entity to DTO
        var responseDto = MapToResponseDto(commentEntity);

        // Cache the result for 5 minutes
        await _cacheService.SetAsync(cacheKey, responseDto, TimeSpan.FromMinutes(5));

        // STEP 15: Return the comment in a success response
        return ApiResponse<GetCommentResponseDto>.OkResponse("Comment retrieved successfully", responseDto);
    }

    // Implement GetCommentsAsync method with pagination and filtering
    // This method retrieves comments from the database with pagination and filtering support
    public async Task<ApiResponse<PageResultResponseDto<GetCommentResponseDto>>> GetCommentsAsync(CommentFilterDto? commentFilter = null)
    {
        // Use default filter if not provided
        var filter = commentFilter ?? new CommentFilterDto();

        // Call the repository with filter
        var commentPageResult = await _commentRepository.GetCommentsWithPaginationAsync(filter);

        // Map entities to DTOs
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

        // Return paginated response
        return ApiResponse<PageResultResponseDto<GetCommentResponseDto>>.OkResponse("Comments retrieved successfully", pageResult);
    }

    // STEP 28: Implement UpdateCommentAsync method
    // This method updates an existing comment
    public async Task<ApiResponse<GetCommentResponseDto>> UpdateCommentAsync(string id, UpdateCommentRequestDto commentUpdate)
    {
        // STEP 29: First, retrieve the existing comment
        var existingComment = await _commentRepository.GetCommentById(id);

        // STEP 30: Check if the comment exists
        if (existingComment == null)
        {
            // STEP 31: Return an error response if comment not found
            return ApiResponse<GetCommentResponseDto>.NotFound("Comment not found");
        }

        // STEP 32: Update only the fields that are provided (partial update)
        if (commentUpdate.Message != null)
            existingComment.Message = commentUpdate.Message;

        // STEP 33: Set the ModifiedOn timestamp
        existingComment.ModifiedOn = DateTime.UtcNow;

        // STEP 34: Call the repository to update the entity
        var result = await _commentRepository.UpdateCommentAsync(existingComment);

        // STEP 35: Check if the update was successful
        if (!result)
        {
            // STEP 36: Return an error response if update failed
            return ApiResponse<GetCommentResponseDto>.InternalServerError();
        }

        // STEP 37: Map the updated entity to DTO
        var responseDto = MapToResponseDto(existingComment);

        // Invalidate cache for this comment
        var cacheKey = $"comment_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        // STEP 38: Return a success response with the updated comment
        return ApiResponse<GetCommentResponseDto>.AcceptedResponse();
    }

    // STEP 39: Implement DeleteCommentByIdAsync method
    // This method deletes a comment by its ID
    public async Task<ApiResponse<bool>> DeleteCommentByIdAsync(string id)
    {
        // STEP 40: First, retrieve the existing comment
        var existingComment = await _commentRepository.GetCommentById(id);

        // STEP 41: Check if the comment exists
        if (existingComment == null)
        {
            // STEP 42: Return an error response if comment not found
            return ApiResponse<bool>.NotFound("Comment not found");
        }

        // STEP 43: Call the repository to delete the entity
        var result = await _commentRepository.DeleteCommentByIdAsync(existingComment);

        // STEP 44: Check if the deletion was successful
        if (!result)
        {
            // STEP 45: Return an error response if deletion failed
            return ApiResponse<bool>.InternalServerError();
        }

        // Invalidate cache for this comment
        var cacheKey = $"comment_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        // STEP 46: Return a success response indicating successful deletion
        return ApiResponse<bool>.NoContent();
    }

    // STEP 47: Create a helper method to map Entity to DTO
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

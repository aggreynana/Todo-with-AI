using System.Linq;
using System;
using Todo.Entities;
using Todo.Model;
using Todo.Model.CommentDto;
using Todo.Model.FilterDto;
using Todo.Services.Interfaces;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Services.Providers;

// STEP 1: Implement the ICommentService interface
// This class contains the business logic for Comment operations
// It acts as a bridge between the controller and the repository layer
public class CommentService : ICommentService
{
    // STEP 2: Inject the ICommentRepository through constructor injection
    // This follows the dependency injection pattern for loose coupling
    private readonly ICommentRepository _commentRepository;

    public CommentService(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    // STEP 3: Implement CreateCommentAsync method
    // This method handles the creation of a new comment
    public async Task<ApiResponse<GetCommentResponseDto>> CreateCommentAsync(CreateCommentRequestDto commentDto)
    {
        // STEP 4: Map the DTO to the entity
        // Convert the incoming DTO to the domain entity for database operations
        var commentEntity = new CommentEntity
        {
            UserId = commentDto.UserId,
            ActivityId = commentDto.ActivityId,
            Message = commentDto.Message
            // Id and CreatedOn are set automatically in BaseEntity
        };

        // STEP 5: Call the repository to add the entity to the database
        var result = await _commentRepository.AddCommentAsync(commentEntity);

        // STEP 6: Check if the operation was successful
        if (!result)
        {
            // STEP 7: Return an error response if creation failed
            return ApiResponse<GetCommentResponseDto>.InternalServerError();
        }

        // STEP 8: Map the entity back to DTO for the response
        var responseDto = MapToResponseDto(commentEntity);

        // STEP 9: Return a success response with the created comment
        return ApiResponse<GetCommentResponseDto>.CreatedResponse("Comment", responseDto);
    }

    // STEP 10: Implement GetCommentByIdAsync method
    // This method retrieves a single comment by its unique identifier
    public async Task<ApiResponse<GetCommentResponseDto>?> GetCommentByIdAsync(string id)
    {
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

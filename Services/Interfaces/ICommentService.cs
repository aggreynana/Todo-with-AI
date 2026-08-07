using Todo.Model;
using Todo.Model.CommentDto;
using Todo.Model.FilterDto;

namespace Todo.Services.Interfaces;

// Define the interface for Comment service
// This interface defines the contract for all business logic operations related to Comment entities
public interface ICommentService
{
    // Create method for adding a new comment
    // Takes a DTO as input to create a new comment
    // Returns a wrapped response with the created comment DTO
    Task<ApiResponse<GetCommentResponseDto>> CreateCommentAsync(CreateCommentRequestDto commentDto);

    // Create method to retrieve a single comment by its ID
    // Returns nullable response since the comment might not exist
    Task<ApiResponse<GetCommentResponseDto>?> GetCommentByIdAsync(string id);

    // Create method to retrieve all comments with pagination and filtering
    // Returns a wrapped response with paginated comment DTOs
    Task<ApiResponse<PageResultResponseDto<GetCommentResponseDto>>> GetCommentsAsync(CommentFilterDto? commentFilter = null);

    // Create method to update an existing comment
    // Takes an update DTO and returns the updated comment DTO
    Task<ApiResponse<GetCommentResponseDto>> UpdateCommentAsync(string id, UpdateCommentRequestDto commentUpdate);

    // Create method to delete a comment by its ID
    // Returns a boolean indicating success/failure
    Task<ApiResponse<bool>> DeleteCommentByIdAsync(string id);
}

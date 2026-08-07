using System.ComponentModel.DataAnnotations;

namespace Todo.Model.CommentDto;

// STEP 1: Create DTO for creating a new comment
// This DTO handles input validation for comment creation
public class CreateCommentRequestDto
{
    // STEP 2: Add required UserId field with validation
    // This links the comment to a specific user
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;

    // STEP 3: Add required ActivityId field with validation
    // This links the comment to a specific activity
    [Required(ErrorMessage = "Activity ID is required")]
    public string ActivityId { get; set; } = string.Empty;

    // STEP 4: Add required Message field with length validation
    [Required(ErrorMessage = "Comment message is required")]
    [StringLength(2000, ErrorMessage = "Comment message cannot exceed 2000 characters")]
    public string Message { get; set; } = string.Empty;
}

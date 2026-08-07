using System.ComponentModel.DataAnnotations;

namespace Todo.Model.CommentDto;

// STEP 1: Create DTO for updating an existing comment
// This DTO handles input validation for comment updates
// All fields are optional to allow partial updates
public class UpdateCommentRequestDto
{
    // STEP 2: Add optional Message field with validation
    [StringLength(2000, ErrorMessage = "Comment message cannot exceed 2000 characters")]
    public string? Message { get; set; }
}

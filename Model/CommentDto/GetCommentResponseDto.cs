namespace Todo.Model.CommentDto;

// STEP 1: Create DTO for returning comment data
// This DTO is used to send comment data back to clients
public class GetCommentResponseDto
{
    // STEP 2: Include the unique identifier
    public string Id { get; set; } = string.Empty;

    // STEP 3: Include user information
    public string UserId { get; set; } = string.Empty;

    // STEP 4: Include activity information
    public string ActivityId { get; set; } = string.Empty;

    // STEP 5: Include comment message
    public string Message { get; set; } = string.Empty;

    // STEP 6: Include tracking timestamps
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}

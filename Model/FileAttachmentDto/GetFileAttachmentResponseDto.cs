namespace Todo.Model.FileAttachmentDto;

// STEP 1: Create DTO for returning file attachment data
// This DTO is used to send file attachment data back to clients
public class GetFileAttachmentResponseDto
{
    // STEP 2: Include the unique identifier
    public string Id { get; set; } = string.Empty;

    // STEP 3: Include user information
    public string UserId { get; set; } = string.Empty;

    // STEP 4: Include activity information
    public string ActivityId { get; set; } = string.Empty;

    // STEP 5: Include file name
    public string FileName { get; set; } = string.Empty;

    // STEP 6: Include file path (can be null for security reasons)
    public string? FilePath { get; set; }

    // STEP 7: Include content type (MIME type)
    public string ContentType { get; set; } = string.Empty;

    // STEP 8: Include upload timestamp
    public DateTime? UpLoadedOn { get; set; }

    // STEP 9: Include tracking timestamps
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}

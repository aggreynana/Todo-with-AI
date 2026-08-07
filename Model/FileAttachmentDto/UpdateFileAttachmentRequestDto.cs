using System.ComponentModel.DataAnnotations;

namespace Todo.Model.FileAttachmentDto;

// STEP 1: Create DTO for updating an existing file attachment
// This DTO handles input validation for file attachment updates
// All fields are optional to allow partial updates
public class UpdateFileAttachmentRequestDto
{
    // STEP 2: Add optional FileName field with validation
    [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters")]
    public string? FileName { get; set; }

    // STEP 3: Add optional FilePath field
    [StringLength(500, ErrorMessage = "File path cannot exceed 500 characters")]
    public string? FilePath { get; set; }

    // STEP 4: Add optional ContentType field with validation
    [StringLength(100, ErrorMessage = "Content type cannot exceed 100 characters")]
    public string? ContentType { get; set; }

    // STEP 5: Add optional UpLoadedOn datetime field
    // Allows updating the upload timestamp
    public DateTime? UpLoadedOn { get; set; }
}

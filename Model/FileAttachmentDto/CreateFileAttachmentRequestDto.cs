using System.ComponentModel.DataAnnotations;

namespace Todo.Model.FileAttachmentDto;

// STEP 1: Create DTO for creating a new file attachment
// This DTO handles input validation for file attachment creation
public class CreateFileAttachmentRequestDto
{
    // STEP 2: Add required UserId field with validation
    // This links the file attachment to a specific user
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;

    // STEP 3: Add required ActivityId field with validation
    // This links the file attachment to a specific activity
    [Required(ErrorMessage = "Activity ID is required")]
    public string ActivityId { get; set; } = string.Empty;

    // STEP 4: Add required FileName field with length validation
    [Required(ErrorMessage = "File name is required")]
    [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters")]
    public string FileName { get; set; } = string.Empty;

    // STEP 5: Add optional FilePath field
    // Stores the physical path or URL where the file is stored
    [StringLength(500, ErrorMessage = "File path cannot exceed 500 characters")]
    public string? FilePath { get; set; }

    // STEP 6: Add required ContentType field with validation
    // Represents the MIME type of the file (e.g., "image/jpeg", "application/pdf")
    [Required(ErrorMessage = "Content type is required")]
    [StringLength(100, ErrorMessage = "Content type cannot exceed 100 characters")]
    public string ContentType { get; set; } = string.Empty;

    // STEP 7: Add optional UpLoadedOn datetime field
    // Allows specifying when the file was uploaded
    public DateTime? UpLoadedOn { get; set; }
}

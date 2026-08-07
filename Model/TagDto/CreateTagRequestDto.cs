using System.ComponentModel.DataAnnotations;

namespace Todo.Model.TagDto;

// STEP 1: Create DTO for creating a new tag
// This DTO handles input validation for tag creation
public class CreateTagRequestDto
{
    // STEP 2: Add required UserId field with validation
    // This links the tag to a specific user
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;

    // STEP 3: Add required Name field with length validation
    [Required(ErrorMessage = "Tag name is required")]
    [StringLength(50, ErrorMessage = "Tag name cannot exceed 50 characters")]
    public string Name { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;

namespace Todo.Model.TagDto;

// STEP 1: Create DTO for updating an existing tag
// This DTO handles input validation for tag updates
// All fields are optional to allow partial updates
public class UpdateTagRequestDto
{
    // STEP 2: Add optional Name field with validation
    [StringLength(50, ErrorMessage = "Tag name cannot exceed 50 characters")]
    public string? Name { get; set; }
}

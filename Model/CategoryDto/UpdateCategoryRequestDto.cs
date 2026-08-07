using System.ComponentModel.DataAnnotations;

namespace Todo.Model.CategoryDto;

// STEP 1: Create DTO for updating an existing category
// This DTO handles input validation for category updates
// All fields are optional to allow partial updates
public class UpdateCategoryRequestDto
{
    // STEP 2: Add optional Name field with validation
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string? Name { get; set; }
}

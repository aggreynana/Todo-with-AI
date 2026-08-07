using System.ComponentModel.DataAnnotations;

namespace Todo.Model.CategoryDto;

// STEP 1: Create DTO for creating a new category
// This DTO handles input validation for category creation
public class CreateCategoryRequestDto
{
    // STEP 2: Add required UserId field with validation
    // This links the category to a specific user
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;

    // STEP 3: Add required Name field with length validation
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;
}

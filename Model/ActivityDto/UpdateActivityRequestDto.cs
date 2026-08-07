using System.ComponentModel.DataAnnotations;
using Todo.Enums;

namespace Todo.Model.ActivityDto;

// STEP 1: Create DTO for updating an existing activity
// This DTO handles input validation for activity updates
// All fields are optional to allow partial updates
public class UpdateActivityRequestDto
{
    // STEP 2: Add optional Title field with validation
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string? Title { get; set; }

    // STEP 3: Add optional Description field
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    // STEP 4: Add optional Status field
    // Allows changing the activity status
    public ActivityStatus? Status { get; set; }

    // STEP 5: Add optional Priority field
    // Allows changing the activity priority
    public ActivityPriority? Priority { get; set; }

    // STEP 6: Add optional CategoryId field
    // Allows moving the activity to a different category
    public string? CategoryId { get; set; }

    // STEP 7: Add optional StartedOn datetime field
    // Allows setting or updating the start time
    public DateTime? StartedOn { get; set; }

    // STEP 8: Add optional EndedOn datetime field
    // Allows setting or updating the end time
    public DateTime? EndedOn { get; set; }
}

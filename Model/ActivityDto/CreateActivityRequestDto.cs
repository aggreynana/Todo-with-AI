using System.ComponentModel.DataAnnotations;
using Todo.Enums;

namespace Todo.Model.ActivityDto;

// STEP 1: Create DTO for creating a new activity
// DTO (Data Transfer Object) is used to transfer data between processes
// This DTO specifically handles input validation for activity creation
public class CreateActivityRequestDto
{
    // STEP 2: Add required UserId field with validation
    // This links the activity to a specific user
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;

    // STEP 3: Add required Title field with length validation
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    // STEP 4: Add optional Description field
    // No required attribute means this field is optional
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    // STEP 5: Add Status field with default value
    // Uses the ActivityStatus enum for type safety
    public ActivityStatus Status { get; set; } = ActivityStatus.Pending;

    // STEP 6: Add Priority field with default value
    // Uses the ActivityPriority enum for type safety
    public ActivityPriority Priority { get; set; } = ActivityPriority.Medium;

    // STEP 7: Add required CategoryId field
    // Links the activity to a specific category
    [Required(ErrorMessage = "Category ID is required")]
    public string CategoryId { get; set; } = string.Empty;

    // STEP 8: Add optional StartedOn datetime field
    // Allows tracking when an activity was started
    public DateTime? StartedOn { get; set; }

    // STEP 9: Add optional EndedOn datetime field
    // Allows tracking when an activity was completed
    public DateTime? EndedOn { get; set; }
}

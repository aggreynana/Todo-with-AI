using Todo.Enums;

namespace Todo.Model.ActivityDto;

// STEP 1: Create DTO for returning activity data
// This DTO is used to send activity data back to clients
// It excludes sensitive information and includes only necessary fields
public class GetActivityResponseDto
{
    // STEP 2: Include the unique identifier
    public string Id { get; set; } = string.Empty;

    // STEP 3: Include user information
    public string UserId { get; set; } = string.Empty;

    // STEP 4: Include activity title
    public string Title { get; set; } = string.Empty;

    // STEP 5: Include optional description
    public string? Description { get; set; }

    // STEP 6: Include activity status
    public ActivityStatus Status { get; set; }

    // STEP 7: Include activity priority
    public ActivityPriority Priority { get; set; }

    // STEP 8: Include category information
    public string CategoryId { get; set; } = string.Empty;

    // STEP 9: Include tracking timestamps
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }

    // STEP 10: Include activity timing information
    public DateTime? StartedOn { get; set; }
    public DateTime? EndedOn { get; set; }

    // STEP 11: Include calculated duration
    // This is a computed property that shows the total time spent on the activity
    public TimeSpan Duration { get; set; }
}

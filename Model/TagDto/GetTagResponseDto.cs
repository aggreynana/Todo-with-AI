namespace Todo.Model.TagDto;

// STEP 1: Create DTO for returning tag data
// This DTO is used to send tag data back to clients
public class GetTagResponseDto
{
    // STEP 2: Include the unique identifier
    public string Id { get; set; } = string.Empty;

    // STEP 3: Include user information
    public string UserId { get; set; } = string.Empty;

    // STEP 4: Include tag name
    public string Name { get; set; } = string.Empty;

    // STEP 5: Include tracking timestamps
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}

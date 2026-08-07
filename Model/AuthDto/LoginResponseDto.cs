namespace Todo.Model.AuthDto;

// STEP 1: Create DTO for login responses
// This DTO returns the JWT token and user information after successful login
public class LoginResponseDto
{
    // STEP 2: Include the JWT token
    // This token will be used for authenticated requests
    public string Token { get; set; } = string.Empty;

    // STEP 3: Include token expiration date
    // Helps the client know when to refresh the token
    public DateTime Expiration { get; set; }

    // STEP 4: Include user information
    // Provides basic user details for the client
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

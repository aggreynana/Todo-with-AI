using System.ComponentModel.DataAnnotations;

namespace Todo.Model.AuthDto;

// STEP 1: Create DTO for login requests
// This DTO handles input validation for user login
public class LoginRequestDto
{
    // STEP 2: Add required Email field with validation
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    // STEP 3: Add required Password field
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}

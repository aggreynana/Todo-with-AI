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
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*()])(.{8,})$", ErrorMessage = "The password must contain at least one upper case, one lower case, one special character, one digit, and has a minimum length of 8 characters")]
    public string Password { get; set; } = string.Empty;
}

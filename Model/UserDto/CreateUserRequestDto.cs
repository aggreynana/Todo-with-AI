using System.ComponentModel.DataAnnotations;

namespace Todo.Model.UserDto;

public class CreateUserRequestDto
{
    [Required(ErrorMessage = "this field is required")]
    [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
    public string FirstName { get; set; } = string.Empty;


    public string? MiddleName { get; set; }


    [Required(ErrorMessage = "this field is required")]
    [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "this field is required")]
    [EmailAddress]
    [MinLength(10)]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;


    [Required(ErrorMessage = "this field is required")]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*()])(.{8,})$", ErrorMessage = "The password must contain at least one upper case, one lower case, one special character, one digit, and has a minimum length of 8 characters")]
    public string Password { get; set; } = string.Empty;
}
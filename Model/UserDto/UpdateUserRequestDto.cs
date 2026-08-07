using System.ComponentModel.DataAnnotations;

namespace Todo.Model.UserDto;

public class UpdateUserRequestDto
{
    [Required(ErrorMessage = "this field is required")]
    [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
    public string FirstName { get; set; } = string.Empty;


    public string? MiddleName { get; set; }


    [Required(ErrorMessage = "this field is required")]
    [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
    public string LastName { get; set; } = string.Empty;

    // Email is not included in update since it's used as an identifier
    // If email updates are needed, a separate method should be created
}
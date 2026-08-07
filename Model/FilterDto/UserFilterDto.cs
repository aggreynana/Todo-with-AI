namespace Todo.Model.FilterDto;

public class UserFilterDto : BaseFilter
{
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; } 
    public string? Email { get; set; }
    public DateTime? CreatedOn { get; set; }
}
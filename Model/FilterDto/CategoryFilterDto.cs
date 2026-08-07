namespace Todo.Model.FilterDto;

public class CategoryFilterDto : BaseFilter
{
    public string? UserId { get; set; }
    public string? Name { get; set; }
}
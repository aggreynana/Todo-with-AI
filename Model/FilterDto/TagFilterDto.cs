namespace Todo.Model.FilterDto;

public class TagFilterDto : BaseFilter
{
    public string? UserId { get; set; }
    public string? Name { get; set; }
}
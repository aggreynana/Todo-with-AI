namespace Todo.Model.FilterDto;

public class CommentFilterDto : BaseFilter
{
    public string? UserId { get; set; }
    public string? ActivityId { get; set; }
    public string? Message { get; set; }
}
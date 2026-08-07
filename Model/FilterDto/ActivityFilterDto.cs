using Todo.Enums;

namespace Todo.Model.FilterDto;

public class ActivityFilterDto : BaseFilter
{
    public string? UserId { get; set; }
    public string? CategoryId { get; set; }
    public string? Title { get; set; }
    public ActivityStatus? Status { get; set; }
    public ActivityPriority? Priority { get; set; }
    public DateTime? StartedOn { get; set; }
    public DateTime? EndedOn { get; set; }
}
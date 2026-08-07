namespace Todo.Entities;

public class CommentEntity : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public UserEntity User { get; set; } = null!;
    public string ActivityId { get; set; } = string.Empty;
    public ActivityEntity Activity { get; set; } = null!;
    public string Message { get; set; } = string.Empty;
}
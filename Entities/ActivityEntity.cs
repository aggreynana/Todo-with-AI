using Todo.Enums;

namespace Todo.Entities;

public class ActivityEntity : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public UserEntity User { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ActivityStatus Status { get; set; }
    public ActivityPriority Priority { get; set; }
    public CategoryEntity Category { get; set; } = null!;
    public string CategoryId { get; set; } = string.Empty;
    public ICollection<CommentEntity> Comments { get; set; } = new List<CommentEntity>();
    public ICollection<TagEntity> Tags { get; set; } = new List<TagEntity>();
    public ICollection<FileAttachmentEntity> FileAttachments { get; set; } = new List<FileAttachmentEntity>();
    public DateTime? StartedOn { get; set; }
    public DateTime? EndedOn { get; set; }

    public TimeSpan Duration => StartedOn.HasValue && EndedOn.HasValue ? EndedOn.Value - StartedOn.Value : TimeSpan.Zero;
}
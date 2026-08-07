namespace Todo.Entities;

public class FileAttachmentEntity : BaseEntity
{
    public DateTime? UpLoadedOn { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public UserEntity User { get; set; } = null!;
    public string ActivityId { get; set; } = string.Empty;
    public ActivityEntity Activity { get; set; } = null!;
}
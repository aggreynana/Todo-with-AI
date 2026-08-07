namespace Todo.Model.FilterDto;

public class FileAttachmentFilterDto : BaseFilter
{
    public string? UserId { get; set; }
    public string? ActivityId { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public DateTime? UpLoadedOn { get; set; }
}
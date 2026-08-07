namespace Todo.Entities;

public class TagEntity : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public UserEntity User { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    
    public ICollection<ActivityEntity> Activities { get; set; } = new List<ActivityEntity>();
}
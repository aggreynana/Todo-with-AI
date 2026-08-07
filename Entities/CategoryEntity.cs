namespace Todo.Entities;

public class CategoryEntity : BaseEntity
{
    public string UserId { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
    public string Name { get; set; } = string.Empty;

    public ICollection<ActivityEntity> Activities { get; set; } = new List<ActivityEntity>();
}
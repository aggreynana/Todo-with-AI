using System.Diagnostics;

namespace Todo.Entities;

// STEP 1: Keep UserEntity as a regular entity without implementing IUser interface
// We'll use IPasswordHasher directly for password hashing
public class UserEntity : BaseEntity
{
    
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // STEP 2: Add PasswordHash property for storing hashed passwords
    public string PasswordHash { get; set; } = string.Empty;

    // STEP 3: Keep the original Password property for backward compatibility
    // This will be used to store the plain text password temporarily before hashing
    [System.Text.Json.Serialization.JsonIgnore]
    public string Password { get; set; } = string.Empty;


    // STEP 5: Keep navigation properties
    public ICollection<TagEntity> Tags { get; set; } = new List<TagEntity>();
    public ICollection<FileAttachmentEntity> FileAttachments { get; set; } = new List<FileAttachmentEntity>();
    public ICollection<CommentEntity> Comments { get; set; } = null!;
    public ICollection<CategoryEntity> Categories { get; set; } = new List<CategoryEntity>();
    public ICollection<ActivityEntity> Activities { get; set; } = new List<ActivityEntity>();
}
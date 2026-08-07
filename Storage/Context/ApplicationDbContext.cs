using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Todo.Entities;

namespace Todo.Storage.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> o) : base(o)
    { }
    
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<TagEntity> Tags { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<ActivityEntity> Activities { get; set; }
    public DbSet<FileAttachmentEntity> FileAttachments { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

}
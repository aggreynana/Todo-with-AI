using Microsoft.EntityFrameworkCore;
using Todo.Entities;

namespace Todo.Storage.Config;

public class TagEntityConfiguration : IEntityTypeConfiguration<TagEntity>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TagEntity> entity)
    {
        entity.HasKey(t => t.Id);
        entity.Property(t => t.Name).IsRequired().HasMaxLength(50);

        entity.HasOne(t => t.User).WithMany(u => u.Tags).HasForeignKey(t => t.UserId);
        entity.HasMany(t => t.Activities).WithMany(a => a.Tags);
    }
}
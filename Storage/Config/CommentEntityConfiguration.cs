using Microsoft.EntityFrameworkCore;
using Todo.Entities;

namespace Todo.Storage.Config;

public class CommentEntityConfiguration : IEntityTypeConfiguration<CommentEntity>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CommentEntity> entity)
    {
        entity.HasKey(cm => cm.Id);
        entity.Property(cm => cm.Message).IsRequired().HasMaxLength(100);

        entity.HasOne(cm => cm.User).WithMany(u => u.Comments).HasForeignKey(cm => cm.UserId);
        entity.HasOne(cm => cm.Activity).WithMany(a => a.Comments).HasForeignKey(cm => cm.ActivityId);
    }
}
using Microsoft.EntityFrameworkCore;
using Todo.Entities;

namespace Todo.Storage.Config;

public class ActivityEntityConfiguration : IEntityTypeConfiguration<ActivityEntity>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ActivityEntity> entity)
    {
        entity.HasKey(a => a.Id);
        entity.Property(a => a.Title).IsRequired().HasMaxLength(50);
        entity.Property(a => a.Description).HasMaxLength(250);
        entity.Property(a => a.CreatedOn).IsRequired().HasColumnType("timestamptz");
        entity.Property(a => a.DeletedOn).HasColumnType("timestamptz");
        entity.Property(a => a.StartedOn).HasColumnType("timestamptz");
        entity.Property(a => a.EndedOn).HasColumnType("timestamptz");
        entity.Property(a => a.ModifiedOn).HasColumnType("timestamptz");
        entity.Property(a => a.Priority).IsRequired().HasConversion<string>();
        entity.Property(a => a.Status).IsRequired().HasConversion<string>();

        entity.HasOne(a => a.User).WithMany(u => u.Activities).HasForeignKey(a => a.UserId);
        entity.HasOne(a => a.Category).WithMany(c => c.Activities).HasForeignKey(a => a.CategoryId);
    }
}
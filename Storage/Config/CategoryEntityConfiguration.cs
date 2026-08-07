using Microsoft.EntityFrameworkCore;
using Todo.Entities;

namespace Todo.Storage.Config;

public class CategoryEntityConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CategoryEntity> entity)
    {
        entity.HasKey(c => c.Id);
        entity.Property(c => c.Name).IsRequired().HasMaxLength(50);

        entity.HasOne(c => c.User).WithMany(u => u.Categories).HasForeignKey(a => a.UserId);
        entity.HasMany(c => c.Activities).WithOne(a => a.Category).HasForeignKey(a => a.CategoryId);
    }
}
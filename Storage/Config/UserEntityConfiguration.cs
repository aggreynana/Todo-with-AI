using Microsoft.EntityFrameworkCore;
using Todo.Entities;

namespace Todo.Storage.Config;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<UserEntity> entity)
    {
        entity.HasKey(u => u.Id);
        entity.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
        entity.Property(u => u.MiddleName).HasMaxLength(50);
        entity.Property(u => u.LastName).IsRequired().HasMaxLength(50);
        entity.Property(u => u.Password).IsRequired().HasMaxLength(255);
        entity.Property(u => u.IsDeleted).IsRequired();
        entity.Property(u => u.CreatedOn).HasColumnType("timestamptz");
        entity.Property(u => u.DeletedOn).HasColumnType("timestamptz");
        entity.Property(u => u.ModifiedOn).HasColumnType("timestamptz");

        entity.HasMany(u => u.Activities).WithOne(u => u.User).HasForeignKey(u => u.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
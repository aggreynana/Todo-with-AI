using Microsoft.EntityFrameworkCore;
using Todo.Entities;

namespace Todo.Storage.Config;

public class FileAttachmentEntityConfiguration : IEntityTypeConfiguration<FileAttachmentEntity>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<FileAttachmentEntity> entity)
    {
        entity.HasKey(fa => fa.Id);
        entity.Property(fa => fa.FileName).IsRequired().HasMaxLength(50);
        entity.Property(fa => fa.FilePath).IsRequired().HasMaxLength(100);
        entity.Property(fa => fa.ContentType).IsRequired().HasMaxLength(20);
        entity.Property(fa => fa.UpLoadedOn).HasColumnType("timestamptz");

        entity.HasOne(fa => fa.User).WithMany(u => u.FileAttachments).HasForeignKey(fa => fa.UserId);
        entity.HasOne(fa => fa.Activity).WithMany(a => a.FileAttachments).HasForeignKey(fa => fa.ActivityId);
        
    }
}
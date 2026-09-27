using MediaService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaService.Infrastructure.Configuration
{


    public sealed class MediaAssetConfiguration
        : IEntityTypeConfiguration<MediaAsset>
    {
        public void Configure(EntityTypeBuilder<MediaAsset> builder)
        {
            builder.ToTable("media_assets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OriginalFileName)
                .HasMaxLength(255);

            builder.Property(x => x.StorageFileName)
                .HasMaxLength(255);

            builder.Property(x => x.StoragePath)
                .HasMaxLength(1000);

            builder.Property(x => x.PublicUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.ContentType)
                .HasMaxLength(100);

            builder.Property(x => x.FileSizeInBytes);

            builder.Property(x => x.MediaType)
                .HasConversion<int>();

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.Property(x => x.FailureReason)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.UploadedByUserId);

            builder.HasIndex(x => x.Status);

            builder.HasIndex(x => x.CreatedAtUtc);
        }
    }
}

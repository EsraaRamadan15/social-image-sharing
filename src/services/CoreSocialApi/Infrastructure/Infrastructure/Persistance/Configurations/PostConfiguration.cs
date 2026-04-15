using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Posts.Domain.Entities;

namespace InfrastructureLibrary.Persistence.Configurations
{
    public sealed class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("posts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Caption)
                .HasMaxLength(2200);

            builder.Property(x => x.Visibility)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.LikeCount)
                .IsRequired();

            builder.Property(x => x.CommentCount)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.HasIndex(x => new { x.UserId, x.CreatedAtUtc });
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Profiles.Domain.Entities;

namespace InfrastructureLibrary.Persistence.Configurations
{
    public sealed class ProfileConfiguration : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder.ToTable("profiles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DisplayName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Bio)
                .HasMaxLength(500);

            builder.Property(x => x.AvatarUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.IsPrivate)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.HasIndex(x => x.UserId)
                .IsUnique();
        }
    }
}

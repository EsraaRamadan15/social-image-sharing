using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SharedInfrastructure.Persistence.Configurations
{
    public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.ToTable("user_sessions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DeviceName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(500);

            builder.Property(x => x.CreatedByIp)
                .HasMaxLength(100);

            builder.Property(x => x.LastSeenIp)
                .HasMaxLength(100);

            builder.Property(x => x.LastSeenAtUtc)
                .IsRequired();

            builder.HasIndex(x => x.UserId);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

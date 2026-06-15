using Application.Abstractions;
using Domain.Common;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Posts.Domain.Entities;
using Profiles.Domain.Entities;

namespace SharedInfrastructure.Persistence
{

    public sealed class AppDbContext : DbContext, IApplicationDbContext
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IDateTimeProvider dateTimeProvider) : base(options)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Profile> Profiles => Set<Profile>();
        public DbSet<Post> Posts => Set<Post>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var utcNow = _dateTimeProvider.UtcNow;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is not AuditableEntity<Guid> auditableEntity)
                    continue;

                if (entry.State == EntityState.Added)
                {
                    auditableEntity.CreatedAtUtc = utcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    auditableEntity.UpdatedAtUtc = utcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}

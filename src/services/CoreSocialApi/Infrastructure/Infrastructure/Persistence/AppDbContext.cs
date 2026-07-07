using Application.Abstractions;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace SharedInfrastructure.Persistence
{
    public sealed class AppDbContext : DbContext, IApplicationDbContext
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IReadOnlyCollection<IModelConfigurationAssembly> _modelConfigurationAssemblies;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IDateTimeProvider dateTimeProvider,
            IEnumerable<IModelConfigurationAssembly> modelConfigurationAssemblies) : base(options)
        {
            _dateTimeProvider = dateTimeProvider;
            _modelConfigurationAssemblies = modelConfigurationAssemblies.ToList();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var assembly in _modelConfigurationAssemblies.Select(x => x.Assembly).Distinct())
            {
                modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            }

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

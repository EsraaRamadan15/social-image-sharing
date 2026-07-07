using Profiles.Application.Abstractions;
using Profiles.Domain.Entities;
using SharedInfrastructure.Persistence;

namespace Profiles.Infrastructure.Repositories
{
    public sealed class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext _dbContext;

        public ProfileRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Profile profile, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<Profile>().AddAsync(profile, cancellationToken);
        }
    }
}

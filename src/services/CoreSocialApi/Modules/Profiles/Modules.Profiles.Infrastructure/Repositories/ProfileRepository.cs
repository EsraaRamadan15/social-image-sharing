using Microsoft.EntityFrameworkCore;
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

        public Task<Profile?> GetByIdAsync(
     Guid id,
     CancellationToken cancellationToken = default)
        {
            return _dbContext.Set<Profile>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public Task<Profile?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.Set<Profile>()
                .FirstOrDefaultAsync(
                    x => x.UserId == userId,
                    cancellationToken);
        }

        public async Task AddAsync(
            Profile profile,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<Profile>()
                .AddAsync(profile, cancellationToken);
        }
    }
}

using Identity.Application.Abstractions.Repositories;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SharedInfrastructure.Persistence;

namespace SharedInfrastructure.Repositories.Identity
{
    public sealed class UserSessionRepository : IUserSessionRepository
    {
        private readonly AppDbContext _dbContext;

        public UserSessionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.UserSessions
                .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);
        }

        public async Task<UserSession?> GetByIdForUserAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.UserSessions
                .FirstOrDefaultAsync(x => x.Id == sessionId && x.UserId == userId, cancellationToken);
        }

        public async Task<IReadOnlyList<UserSession>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.UserSessions
                .Where(x => x.UserId == userId && !x.RevokedAtUtc.HasValue)
                .OrderByDescending(x => x.LastSeenAtUtc)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(UserSession session, CancellationToken cancellationToken = default)
        {
            await _dbContext.UserSessions.AddAsync(session, cancellationToken);
        }
    }
}

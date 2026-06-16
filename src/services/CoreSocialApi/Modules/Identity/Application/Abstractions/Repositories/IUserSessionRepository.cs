using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Repositories
{
    public interface IUserSessionRepository
    {
        Task<UserSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);

        Task<UserSession?> GetByIdForUserAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<UserSession>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task AddAsync(UserSession session, CancellationToken cancellationToken = default);
    }
}

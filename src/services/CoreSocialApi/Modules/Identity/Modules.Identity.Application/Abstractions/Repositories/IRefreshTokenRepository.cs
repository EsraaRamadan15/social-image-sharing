using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RefreshToken>> ListActiveBySessionIdAsync(Guid sessionId, DateTimeOffset now, CancellationToken cancellationToken = default);
        Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    }
}

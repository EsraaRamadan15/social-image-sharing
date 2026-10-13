using Profiles.Domain.Entities;

namespace Profiles.Application.Abstractions
{
    public interface IProfileRepository
    {
        Task<Profile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Profile?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Profile profile,
            CancellationToken cancellationToken = default);
    }
}

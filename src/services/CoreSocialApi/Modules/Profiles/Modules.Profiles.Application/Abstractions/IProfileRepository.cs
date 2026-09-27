using Profiles.Domain.Entities;

namespace Profiles.Application.Abstractions
{
    public interface IProfileRepository
    {
        Task AddAsync(Profile profile, CancellationToken cancellationToken = default);
    }
}

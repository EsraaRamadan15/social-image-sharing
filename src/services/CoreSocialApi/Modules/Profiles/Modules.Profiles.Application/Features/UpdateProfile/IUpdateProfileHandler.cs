using Application.Common;
using Profiles.Application.Features.UpdateProfile.Models;

namespace Profiles.Application.Features.UpdateProfile
{
    public interface IUpdateProfileHandler
    {
        Task<Result> HandleAsync(
            UpdateProfileRequest request,
            CancellationToken cancellationToken = default);
    }
}

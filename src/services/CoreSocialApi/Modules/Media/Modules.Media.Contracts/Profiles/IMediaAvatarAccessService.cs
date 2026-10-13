using MediaService.Contracts.Profiles.Models;

namespace MediaService.Contracts.Profiles
{
    public interface IMediaAvatarAccessService
    {
        Task<MediaAvatarAccessResult> CanUseAsAvatarAsync(
            Guid mediaId,
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}

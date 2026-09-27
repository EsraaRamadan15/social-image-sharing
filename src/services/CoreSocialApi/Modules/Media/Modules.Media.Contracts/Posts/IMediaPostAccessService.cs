using MediaService.Contracts.Posts.Models;

namespace MediaService.Contracts.Posts
{

    public interface IMediaPostAccessService
    {
        Task<MediaPostAccessResult> CanAttachToPostAsync(
            Guid mediaId,
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}

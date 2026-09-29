using MediaService.Contracts.Posts.Models;

namespace MediaService.Contracts.Posts
{
    public interface IMediaPostQueryService
    {
        Task<MediaPostInfo?> GetForPostAsync(
            Guid mediaId,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<Guid, MediaPostInfo>> GetForPostsAsync(
       IReadOnlyCollection<Guid> mediaIds,
       CancellationToken cancellationToken = default);
    }
}

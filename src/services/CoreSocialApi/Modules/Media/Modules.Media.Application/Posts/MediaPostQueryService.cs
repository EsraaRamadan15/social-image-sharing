using MediaService.Application.Abstractions;
using MediaService.Contracts.Posts;
using MediaService.Contracts.Posts.Models;
using MediaService.Domain.Enums;

namespace MediaService.Application.Posts
{
    public sealed class MediaPostQueryService
    : IMediaPostQueryService
    {
        private readonly IMediaAssetRepository _mediaRepository;

        public MediaPostQueryService(
            IMediaAssetRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        public async Task<MediaPostInfo?> GetForPostAsync(
            Guid mediaId,
            CancellationToken cancellationToken = default)
        {
            var media = await _mediaRepository.GetByIdAsync(
                mediaId,
                cancellationToken);

            if (media is null)
                return null;

            if (media.Status != MediaStatus.Ready)
                return null;

            if (string.IsNullOrWhiteSpace(media.PublicUrl))
                return null;

            return new MediaPostInfo(
                media.Id,
                media.PublicUrl,
                media.ContentType ?? "application/octet-stream");
        }


        public async Task<IReadOnlyDictionary<Guid, MediaPostInfo>>
    GetForPostsAsync(
        IReadOnlyCollection<Guid> mediaIds,
        CancellationToken cancellationToken = default)
        {
            if (mediaIds.Count == 0)
            {
                return new Dictionary<Guid, MediaPostInfo>();
            }

            var uniqueIds = mediaIds
                .Distinct()
                .ToArray();

            var mediaAssets = await _mediaRepository.GetByIdsAsync(
                uniqueIds,
                cancellationToken);

            return mediaAssets
                .Where(x =>
                    x.Status == MediaStatus.Ready &&
                    !string.IsNullOrWhiteSpace(x.PublicUrl))
                .ToDictionary(
                    x => x.Id,
                    x => new MediaPostInfo(
                        x.Id,
                        x.PublicUrl!,
                        x.ContentType ?? "application/octet-stream"));
        }
    }
}

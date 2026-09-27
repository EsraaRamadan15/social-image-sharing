using MediaService.Application.Abstractions;
using MediaService.Contracts.Posts;
using MediaService.Contracts.Posts.Models;
using MediaService.Domain.Enums;

namespace Modules.Media.Application.Posts;

public sealed class MediaPostAccessService : IMediaPostAccessService
{
    private readonly IMediaAssetRepository _mediaRepository;

    public MediaPostAccessService(
        IMediaAssetRepository mediaRepository)
    {
        _mediaRepository = mediaRepository;
    }

    public async Task<MediaPostAccessResult> CanAttachToPostAsync(
        Guid mediaId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var media = await _mediaRepository.GetByIdAsync(
            mediaId,
            cancellationToken);

        if (media is null || media.Status == MediaStatus.Deleted)
        {
            return new MediaPostAccessResult(
                false,
                MediaPostAccessFailure.NotFound);
        }

        if (media.UploadedByUserId != userId)
        {
            return new MediaPostAccessResult(
                false,
                MediaPostAccessFailure.NotOwnedByUser);
        }

        if (media.Status != MediaStatus.Ready)
        {
            return new MediaPostAccessResult(
                false,
                MediaPostAccessFailure.NotReady);
        }

        if (media.MediaType != MediaType.Image)
        {
            return new MediaPostAccessResult(
                false,
                MediaPostAccessFailure.UnsupportedMediaType);
        }

        return new MediaPostAccessResult(
            true,
            null);
    }
}
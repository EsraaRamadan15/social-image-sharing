using Application.Common;
using MediaService.Application.Abstractions;
using MediaService.Application.Errors;
using MediaService.Domain.Enums;

namespace MediaService.Application.Features.GetMedia
{

    public sealed class GetMediaHandler : IGetMediaHandler
    {
        private readonly IMediaAssetRepository _repository;

        public GetMediaHandler(IMediaAssetRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<GetMediaResponse>> HandleAsync(
            Guid mediaId,
            CancellationToken cancellationToken = default)
        {
            var mediaAsset = await _repository.GetByIdAsync(mediaId, cancellationToken);

            if (mediaAsset is null || mediaAsset.Status == MediaStatus.Deleted)
                return Result<GetMediaResponse>.Failure(MediaErrors.NotFound);

            return Result<GetMediaResponse>.Success(new GetMediaResponse
            {
                MediaId = mediaAsset.Id,
                OriginalFileName = mediaAsset.OriginalFileName,
                PublicUrl = mediaAsset.PublicUrl,
                ContentType = mediaAsset.ContentType,
                FileSizeInBytes = mediaAsset.FileSizeInBytes,
                MediaType = mediaAsset.MediaType,
                Status = mediaAsset.Status,
                CreatedAtUtc = mediaAsset.CreatedAtUtc
            });
        }
    }
}

using Application.Abstractions;
using Application.Common;
using MediaService.Application.Abstractions;
using MediaService.Application.Errors;
using MediaService.Application.Options;
using Microsoft.Extensions.Options;

namespace MediaService.Application.Features.UploadContent
{
    public sealed class UploadContentHandler : IUploadContentHandler
    {
        private readonly IMediaAssetRepository _repository;
        private readonly IFileStorage _fileStorage;
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly MediaUploadOptions _options;

        public UploadContentHandler(
            IMediaAssetRepository repository,
            IFileStorage fileStorage,
            IApplicationDbContext dbContext,
            ICurrentUserService currentUserService,
            IDateTimeProvider dateTimeProvider,
            IOptions<MediaUploadOptions> options)
        {
            _repository = repository;
            _fileStorage = fileStorage;
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _dateTimeProvider = dateTimeProvider;
            _options = options.Value;
        }

        public async Task<Result<UploadContentResponse>> HandleAsync(
            UploadContentRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
            {
                return Result<UploadContentResponse>.Failure(
                    new Error("auth.unauthorized", "User must be authenticated."));
            }

            var mediaAsset = await _repository.GetByIdAsync(request.MediaId, cancellationToken);

            if (mediaAsset is null)
                return Result<UploadContentResponse>.Failure(MediaErrors.NotFound);

            if (mediaAsset.UploadedByUserId != _currentUserService.UserId.Value)
                return Result<UploadContentResponse>.Failure(MediaErrors.UploadNotAllowed);

            if (request.FileStream == Stream.Null || request.FileSizeInBytes <= 0)
                return Result<UploadContentResponse>.Failure(MediaErrors.InvalidFile);

            if (request.FileSizeInBytes > _options.MaxFileSizeInBytes)
                return Result<UploadContentResponse>.Failure(MediaErrors.FileTooLarge);

            if (!_options.AllowedContentTypes.Contains(request.ContentType, StringComparer.OrdinalIgnoreCase))
                return Result<UploadContentResponse>.Failure(MediaErrors.UnsupportedFileType);

            try
            {
                var now = _dateTimeProvider.UtcNow;

                mediaAsset.StartUpload(now);

                var storedFile = await _fileStorage.SaveAsync(
                    request.FileStream,
                    request.OriginalFileName,
                    request.ContentType,
                    cancellationToken);

                mediaAsset.MarkUploaded(
                    request.OriginalFileName,
                    storedFile.StorageFileName,
                    storedFile.StoragePath,
                    storedFile.PublicUrl,
                    request.ContentType,
                    storedFile.SizeInBytes,
                    _dateTimeProvider.UtcNow);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return Result<UploadContentResponse>.Success(new UploadContentResponse
                {
                    MediaId = mediaAsset.Id,
                    PublicUrl = mediaAsset.PublicUrl!,
                    Status = mediaAsset.Status
                });
            }
            catch (Exception ex)
            {
                mediaAsset.MarkFailed(ex.Message, _dateTimeProvider.UtcNow);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return Result<UploadContentResponse>.Failure(MediaErrors.InvalidFile);
            }
        }
    }
}

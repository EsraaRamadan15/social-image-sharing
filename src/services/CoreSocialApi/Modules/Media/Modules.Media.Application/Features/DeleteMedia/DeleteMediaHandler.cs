using Application.Abstractions;
using Application.Common;
using MediaService.Application.Abstractions;
using MediaService.Application.Errors;
using MediaService.Domain.Enums;

namespace MediaService.Application.Features.DeleteMedia
{

    public sealed class DeleteMediaHandler : IDeleteMediaHandler
    {
        private readonly IMediaAssetRepository _repository;
        private readonly IFileStorage _fileStorage;
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public DeleteMediaHandler(
            IMediaAssetRepository repository,
            IFileStorage fileStorage,
            IApplicationDbContext dbContext,
            ICurrentUserService currentUserService,
            IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _fileStorage = fileStorage;
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result> HandleAsync(
            Guid mediaId,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
            {
                return Result.Failure(new Error("auth.unauthorized", "User must be authenticated."));
            }

            var mediaAsset = await _repository.GetByIdAsync(mediaId, cancellationToken);

            if (mediaAsset is null || mediaAsset.Status == MediaStatus.Deleted)
                return Result.Failure(MediaErrors.NotFound);

            if (mediaAsset.UploadedByUserId != _currentUserService.UserId.Value)
                return Result.Failure(MediaErrors.UploadNotAllowed);

            if (!string.IsNullOrWhiteSpace(mediaAsset.StoragePath))
            {
                await _fileStorage.DeleteAsync(mediaAsset.StoragePath, cancellationToken);
            }

            mediaAsset.Delete(_dateTimeProvider.UtcNow);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

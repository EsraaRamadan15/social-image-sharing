using Application.Abstractions;
using Application.Common;
using MediaService.Application.Abstractions;
using MediaService.Domain.Entities;

namespace MediaService.Application.Features.CreateUploadSession
{


    public sealed class CreateUploadSessionHandler : ICreateUploadSessionHandler
    {
        private readonly IMediaAssetRepository _repository;
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CreateUploadSessionHandler(
            IMediaAssetRepository repository,
            IApplicationDbContext dbContext,
            ICurrentUserService currentUserService,
            IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result<CreateUploadSessionResponse>> HandleAsync(
            CreateUploadSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
            {
                return Result<CreateUploadSessionResponse>.Failure(
                    new Error("auth.unauthorized", "User must be authenticated."));
            }

            var mediaAsset = MediaAsset.CreateUploadSession(
                Guid.NewGuid(),
                _currentUserService.UserId.Value,
                request.MediaType,
                _dateTimeProvider.UtcNow);

            await _repository.AddAsync(mediaAsset, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CreateUploadSessionResponse>.Success(new CreateUploadSessionResponse
            {
                MediaId = mediaAsset.Id,
                Status = mediaAsset.Status
            });
        }
    }
}

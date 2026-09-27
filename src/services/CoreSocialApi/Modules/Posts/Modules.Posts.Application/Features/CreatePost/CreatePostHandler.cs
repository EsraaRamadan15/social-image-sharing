using Application.Abstractions;
using Application.Common;
using MediaService.Contracts.Posts;
using MediaService.Contracts.Posts.Models;
using Posts.Application.Abstractions;
using Posts.Application.Errors;
using Posts.Domain.Entities;

namespace Posts.Application.Features.CreatePost
{

    public sealed class CreatePostHandler : ICreatePostHandler
    {
        private const int MaxCaptionLength = 2200;

        private readonly IPostRepository _postRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMediaPostAccessService _mediaAccessService;

        public CreatePostHandler(
            IPostRepository postRepository,
            IApplicationDbContext dbContext,
            ICurrentUserService currentUserService,
            IDateTimeProvider dateTimeProvider,
            IMediaPostAccessService mediaAccessService)
        {
            _postRepository = postRepository;
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _dateTimeProvider = dateTimeProvider;
            _mediaAccessService = mediaAccessService;
        }

        public async Task<Result<CreatePostResponse>> HandleAsync(
            CreatePostRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
                return Result<CreatePostResponse>.Failure(PostErrors.Unauthorized);

            if (request.MediaId == Guid.Empty)
                return Result<CreatePostResponse>.Failure(PostErrors.InvalidMedia);

            if (!string.IsNullOrWhiteSpace(request.Caption) &&
                request.Caption.Length > MaxCaptionLength)
                return Result<CreatePostResponse>.Failure(PostErrors.CaptionTooLong);

            var now = _dateTimeProvider.UtcNow;
            var userId = _currentUserService.UserId!.Value;

            var mediaAccess = await _mediaAccessService.CanAttachToPostAsync(
                request.MediaId,
                userId,
                cancellationToken);

            if (!mediaAccess.IsAllowed)
            {
                return Result<CreatePostResponse>.Failure(
                    MapMediaError(mediaAccess.Failure));
            }

            var post = Post.Create(
                Guid.NewGuid(),
                _currentUserService.UserId.Value,
                request.MediaId,
                request.Caption,
                request.Visibility,
                now);

            await _postRepository.AddAsync(post, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CreatePostResponse>.Success(new CreatePostResponse
            {
                PostId = post.Id,
                MediaId = post.MediaId,
                Caption = post.Caption,
                Visibility = post.Visibility,
                CreatedAtUtc = post.CreatedAtUtc
            });
        }
        private static Error MapMediaError(
    MediaPostAccessFailure? failure)
        {
            return failure switch
            {
                MediaPostAccessFailure.NotFound =>
                    PostErrors.MediaNotFound,

                MediaPostAccessFailure.NotOwnedByUser =>
                    PostErrors.MediaNotOwned,

                MediaPostAccessFailure.NotReady =>
                    PostErrors.MediaNotReady,

                MediaPostAccessFailure.UnsupportedMediaType =>
                    PostErrors.UnsupportedMedia,

                _ =>
                    PostErrors.InvalidMedia
            };
        }
    }
}

using Application.Abstractions;
using Application.Common;
using Posts.Application.Abstractions;
using Posts.Application.Errors;

namespace Posts.Application.Features.DeletePost
{

    public sealed class DeletePostHandler : IDeletePostHandler
    {
        private readonly IPostRepository _postRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public DeletePostHandler(
            IPostRepository postRepository,
            IApplicationDbContext dbContext,
            ICurrentUserService currentUserService)
        {
            _postRepository = postRepository;
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<Result> HandleAsync(
            Guid postId,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
                return Result.Failure(PostErrors.Unauthorized);

            var post = await _postRepository.GetByIdAsync(postId, cancellationToken);

            if (post is null)
                return Result.Failure(PostErrors.NotFound);

            var isOwner = post.UserId == _currentUserService.UserId.Value;

            if (!isOwner)
                return Result.Failure(PostErrors.Unauthorized);

            // For now hard delete via repository will be added in next step.
            // Better later: soft delete with Status.
            _postRepository.Remove(post);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

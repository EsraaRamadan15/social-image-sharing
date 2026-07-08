using Application.Common;
using Posts.Application.Abstractions;
using Posts.Application.Errors;

namespace Posts.Application.Features.GetPost
{


    public sealed class GetPostHandler : IGetPostHandler
    {
        private readonly IPostRepository _postRepository;

        public GetPostHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<Result<GetPostResponse>> HandleAsync(
            Guid postId,
            CancellationToken cancellationToken = default)
        {
            var post = await _postRepository.GetByIdAsync(postId, cancellationToken);

            if (post is null)
                return Result<GetPostResponse>.Failure(PostErrors.NotFound);

            return Result<GetPostResponse>.Success(new GetPostResponse
            {
                PostId = post.Id,
                UserId = post.UserId,
                MediaId = post.MediaId,
                Caption = post.Caption,
                Visibility = post.Visibility,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                CreatedAtUtc = post.CreatedAtUtc
            });
        }
    }
}

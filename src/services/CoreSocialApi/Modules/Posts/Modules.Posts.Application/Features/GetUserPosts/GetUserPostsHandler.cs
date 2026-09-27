using Application.Common;
using Posts.Application.Abstractions;
using Posts.Application.Features.GetPost;

namespace Posts.Application.Features.GetUserPosts
{
    public sealed class GetUserPostsHandler : IGetUserPostsHandler
    {
        private readonly IPostRepository _postRepository;

        public GetUserPostsHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<Result<GetUserPostsResponse>> HandleAsync(
            Guid userId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 50);

            var posts = await _postRepository.GetUserPostsAsync(
                userId,
                page,
                pageSize,
                cancellationToken);

            return Result<GetUserPostsResponse>.Success(new GetUserPostsResponse
            {
                Items = posts.Select(post => new GetPostResponse
                {
                    PostId = post.Id,
                    UserId = post.UserId,
                    MediaId = post.MediaId,
                    Caption = post.Caption,
                    Visibility = post.Visibility,
                    LikeCount = post.LikeCount,
                    CommentCount = post.CommentCount,
                    CreatedAtUtc = post.CreatedAtUtc
                }).ToList()
            });
        }
    }
}

using Application.Common;

namespace Posts.Application.Features.GetUserPosts
{
    public interface IGetUserPostsHandler
    {
        Task<Result<GetUserPostsResponse>> HandleAsync(
            Guid userId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}

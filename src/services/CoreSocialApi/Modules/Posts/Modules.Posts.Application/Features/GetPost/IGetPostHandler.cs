using Application.Common;
using Posts.Application.Features.GetPost.Models;

namespace Posts.Application.Features.GetPost
{
    public interface IGetPostHandler
    {
        Task<Result<GetPostResponse>> HandleAsync(
            Guid postId,
            CancellationToken cancellationToken = default);
    }
}

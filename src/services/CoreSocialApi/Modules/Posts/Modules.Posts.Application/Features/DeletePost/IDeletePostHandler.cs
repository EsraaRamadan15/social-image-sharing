using Application.Common;

namespace Posts.Application.Features.DeletePost
{

    public interface IDeletePostHandler
    {
        Task<Result> HandleAsync(
            Guid postId,
            CancellationToken cancellationToken = default);
    }
}

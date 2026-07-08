using Application.Common;

namespace Posts.Application.Features.CreatePost
{
    public interface ICreatePostHandler
    {
        Task<Result<CreatePostResponse>> HandleAsync(
            CreatePostRequest request,
            CancellationToken cancellationToken = default);
    }
}

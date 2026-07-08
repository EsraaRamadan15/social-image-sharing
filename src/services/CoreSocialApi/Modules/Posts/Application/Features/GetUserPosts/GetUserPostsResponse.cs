using Posts.Application.Features.GetPost;

namespace Posts.Application.Features.GetUserPosts
{
    public sealed class GetUserPostsResponse
    {
        public IReadOnlyCollection<GetPostResponse> Items { get; init; } = [];
    }
}

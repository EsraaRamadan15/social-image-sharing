using Posts.Application.Features.GetPost.Models;

namespace Posts.Application.Features.GetUserPosts
{
    public sealed class GetUserPostsResponse
    {
        public IReadOnlyCollection<GetPostResponse> Items { get; init; } = [];
    }
}

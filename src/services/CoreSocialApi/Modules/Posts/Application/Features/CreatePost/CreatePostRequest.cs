using Posts.Domain.Enums;

namespace Posts.Application.Features.CreatePost
{
    public sealed class CreatePostRequest
    {
        public Guid MediaId { get; init; }
        public string? Caption { get; init; }
        public PostVisibility Visibility { get; init; } = PostVisibility.Public;
    }
}

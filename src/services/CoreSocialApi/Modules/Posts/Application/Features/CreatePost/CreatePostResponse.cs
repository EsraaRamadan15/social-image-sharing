using Posts.Domain.Enums;

namespace Posts.Application.Features.CreatePost
{
    public sealed class CreatePostResponse
    {
        public Guid PostId { get; init; }
        public Guid MediaId { get; init; }
        public string? Caption { get; init; }
        public PostVisibility Visibility { get; init; }
        public DateTimeOffset CreatedAtUtc { get; init; }
    }
}

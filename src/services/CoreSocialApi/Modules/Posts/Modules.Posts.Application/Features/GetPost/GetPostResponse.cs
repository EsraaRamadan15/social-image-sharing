using Posts.Domain.Enums;

namespace Posts.Application.Features.GetPost
{
    public sealed class GetPostResponse
    {
        public Guid PostId { get; init; }
        public Guid UserId { get; init; }
        public Guid MediaId { get; init; }
        public string? Caption { get; init; }
        public PostVisibility Visibility { get; init; }
        public int LikeCount { get; init; }
        public int CommentCount { get; init; }
        public DateTimeOffset CreatedAtUtc { get; init; }
    }
}

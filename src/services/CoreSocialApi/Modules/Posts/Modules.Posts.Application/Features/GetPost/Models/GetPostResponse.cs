using Posts.Domain.Enums;

namespace Posts.Application.Features.GetPost.Models
{
    public sealed class GetPostResponse
    {
        public Guid PostId { get; init; }
        public Guid UserId { get; init; }
        public PostMediaResponse? Media { get; init; }
        public string? Caption { get; init; }
        public PostVisibility Visibility { get; init; }
        public int LikeCount { get; init; }
        public int CommentCount { get; init; }
        public DateTimeOffset CreatedAtUtc { get; init; }
    }
}

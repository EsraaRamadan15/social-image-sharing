using Domain.Common;
using Posts.Domain.Enums;

namespace Posts.Domain.Entities
{
    public sealed class Post : AuditableEntity<Guid>
    {
        private Post()
        {
        }

        public Guid UserId { get; private set; }
        public Guid MediaId { get; private set; }
        public string? Caption { get; private set; }
        public PostVisibility Visibility { get; private set; }
        public int LikeCount { get; private set; }
        public int CommentCount { get; private set; }

        public static Post Create(
            Guid id,
            Guid userId,
            Guid mediaId,
            string? caption,
            PostVisibility visibility,
            DateTimeOffset createdAtUtc)
        {
            return new Post
            {
                Id = id,
                UserId = userId,
                MediaId = mediaId,
                Caption = caption,
                Visibility = visibility,
                LikeCount = 0,
                CommentCount = 0,
                CreatedAtUtc = createdAtUtc
            };
        }

        public void UpdateCaption(string? caption, PostVisibility visibility, DateTimeOffset updatedAtUtc)
        {
            Caption = caption;
            Visibility = visibility;
            UpdatedAtUtc = updatedAtUtc;
        }

        public void IncrementLikeCount()
        {
            LikeCount++;
        }

        public void DecrementLikeCount()
        {
            if (LikeCount > 0)
                LikeCount--;
        }

        public void IncrementCommentCount()
        {
            CommentCount++;
        }

        public void DecrementCommentCount()
        {
            if (CommentCount > 0)
                CommentCount--;
        }
    }
}

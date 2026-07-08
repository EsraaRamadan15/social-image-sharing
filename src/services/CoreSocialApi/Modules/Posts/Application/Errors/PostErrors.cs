using Application.Common;

namespace Posts.Application.Errors
{

    public static class PostErrors
    {
        public static readonly Error NotFound =
            new("posts.not_found", "Post was not found.");

        public static readonly Error Unauthorized =
            new("posts.unauthorized", "You are not allowed to perform this action.");

        public static readonly Error InvalidMedia =
            new("posts.invalid_media", "The selected media is invalid.");

        public static readonly Error CaptionTooLong =
            new("posts.caption_too_long", "Caption is too long.");
    }
}

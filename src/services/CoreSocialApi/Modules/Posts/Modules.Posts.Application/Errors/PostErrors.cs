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
        public static readonly Error MediaNotFound =
    new(
        "posts.media_not_found",
        "The selected media was not found.");

        public static readonly Error MediaNotOwned =
            new(
                "posts.media_not_owned",
                "The selected media does not belong to the current user.");

        public static readonly Error MediaNotReady =
            new(
                "posts.media_not_ready",
                "The selected media is not ready.");

        public static readonly Error UnsupportedMedia =
            new(
                "posts.unsupported_media",
                "The selected media type cannot be attached to a post.");
    }
}

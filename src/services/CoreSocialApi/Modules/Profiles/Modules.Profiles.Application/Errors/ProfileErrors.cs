using Application.Common;

namespace Profiles.Application.Errors
{
    public static class ProfileErrors
    {
        public static readonly Error NotFound =
            new(
                "profiles.not_found",
                "Profile was not found.");

        public static readonly Error Unauthorized =
            new(
                "profiles.unauthorized",
                "You are not allowed to modify this profile.");

        public static readonly Error InvalidDisplayName =
            new(
                "profiles.invalid_display_name",
                "Display name is invalid.");

        public static readonly Error BioTooLong =
            new(
                "profiles.bio_too_long",
                "Bio is too long.");

        public static readonly Error InvalidAvatar =
            new(
                "profiles.invalid_avatar",
                "The selected avatar is invalid.");
    }
}

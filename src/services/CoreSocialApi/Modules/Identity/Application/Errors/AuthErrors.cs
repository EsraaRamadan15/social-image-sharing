
using Application.Common;

namespace Identity.Application.Errors
{
    public static class AuthErrors
    {
        public static readonly Error InvalidCredentials =
            new("auth.invalid_credentials", "Invalid email or password.");

        public static readonly Error EmailAlreadyExists =
            new("auth.email_already_exists", "Email is already in use.");

        public static readonly Error UserNameAlreadyExists =
            new("auth.username_already_exists", "Username is already in use.");

        public static readonly Error UserInactive =
            new("auth.user_inactive", "The user account is inactive.");

        public static readonly Error InvalidRefreshToken =
            new("auth.invalid_refresh_token", "The refresh token is invalid.");

        public static readonly Error ExpiredRefreshToken =
            new("auth.expired_refresh_token", "The refresh token has expired.");

        public static readonly Error InvalidSession =
            new("auth.invalid_session", "The session is invalid.");
    }
}

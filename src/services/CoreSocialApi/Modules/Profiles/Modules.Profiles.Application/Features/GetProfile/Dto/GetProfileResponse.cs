namespace Profiles.Application.Features.GetProfile.Dto
{
    public sealed class GetProfileResponse
    {
        public Guid UserId { get; init; }

        public string DisplayName { get; init; } = string.Empty;

        public string? Bio { get; init; }

        public bool IsPrivate { get; init; }

        public ProfileAvatarResponse? Avatar { get; init; }
    }

    public sealed class ProfileAvatarResponse
    {
        public Guid MediaId { get; init; }

        public string Url { get; init; } = string.Empty;
    }
}

namespace Profiles.Application.Features.UpdateProfile.Models
{
    public sealed class UpdateProfileRequest
    {
        public string DisplayName { get; init; } = string.Empty;

        public string? Bio { get; init; }
    }
}

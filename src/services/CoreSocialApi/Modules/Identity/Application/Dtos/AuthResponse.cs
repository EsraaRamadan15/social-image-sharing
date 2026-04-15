namespace Identity.Application.Dtos
{
    public sealed class AuthResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public DateTimeOffset ExpiresAtUtc { get; init; }
    }
}

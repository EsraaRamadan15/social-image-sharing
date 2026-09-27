namespace Identity.Application.Dtos
{
    public sealed class UserSessionResponse
    {
        public Guid Id { get; init; }
        public string DeviceName { get; init; } = string.Empty;
        public string? UserAgent { get; init; }
        public string? CreatedByIp { get; init; }
        public string? LastSeenIp { get; init; }
        public DateTimeOffset CreatedAtUtc { get; init; }
        public DateTimeOffset LastSeenAtUtc { get; init; }
        public bool IsCurrent { get; init; }
    }
}

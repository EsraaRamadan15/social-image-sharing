using Domain.Common;

namespace Identity.Domain.Entities
{
    public sealed class UserSession : AuditableEntity<Guid>
    {
        private UserSession()
        {
        }

        public Guid UserId { get; private set; }
        public string DeviceName { get; private set; } = string.Empty;
        public string? UserAgent { get; private set; }
        public string? CreatedByIp { get; private set; }
        public string? LastSeenIp { get; private set; }
        public DateTimeOffset LastSeenAtUtc { get; private set; }
        public DateTimeOffset? RevokedAtUtc { get; private set; }

        public bool IsRevoked => RevokedAtUtc.HasValue;

        public static UserSession Create(
            Guid id,
            Guid userId,
            string? deviceName,
            string? userAgent,
            string? createdByIp,
            DateTimeOffset createdAtUtc)
        {
            return new UserSession
            {
                Id = id,
                UserId = userId,
                DeviceName = string.IsNullOrWhiteSpace(deviceName) ? "Unknown device" : deviceName.Trim(),
                UserAgent = userAgent,
                CreatedByIp = createdByIp,
                LastSeenIp = createdByIp,
                LastSeenAtUtc = createdAtUtc,
                CreatedAtUtc = createdAtUtc
            };
        }

        public void MarkSeen(DateTimeOffset seenAtUtc, string? ipAddress)
        {
            LastSeenAtUtc = seenAtUtc;
            LastSeenIp = ipAddress;
            UpdatedAtUtc = seenAtUtc;
        }

        public void Revoke(DateTimeOffset revokedAtUtc)
        {
            RevokedAtUtc = revokedAtUtc;
            UpdatedAtUtc = revokedAtUtc;
        }
    }
}

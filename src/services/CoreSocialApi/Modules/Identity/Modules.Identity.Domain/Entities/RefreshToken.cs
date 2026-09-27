using Domain.Common;

namespace Identity.Domain.Entities
{
    public sealed class RefreshToken : AuditableEntity<Guid>
    {
        private RefreshToken()
        {
        }

        public Guid UserId { get; private set; }
        public Guid SessionId { get; private set; }
        public string TokenHash { get; private set; } = string.Empty;
        public DateTimeOffset ExpiresAtUtc { get; private set; }
        public DateTimeOffset? RevokedAtUtc { get; private set; }
        public string? ReplacedByToken { get; private set; }
        public string? CreatedByIp { get; private set; }

        public bool IsRevoked => RevokedAtUtc.HasValue;

        public bool IsExpiredAt(DateTimeOffset now) => ExpiresAtUtc <= now;

        public bool IsActiveAt(DateTimeOffset now) => !IsRevoked && !IsExpiredAt(now);

        public static RefreshToken Create(
            Guid id,
            Guid userId,
            Guid sessionId,
            string tokenHash,
            DateTimeOffset expiresAtUtc,
            DateTimeOffset createdAtUtc,
            string? createdByIp)
        {
            return new RefreshToken
            {
                Id = id,
                UserId = userId,
                SessionId = sessionId,
                TokenHash = tokenHash,
                ExpiresAtUtc = expiresAtUtc,
                CreatedAtUtc = createdAtUtc,
                CreatedByIp = createdByIp
            };
        }

        public void Revoke(DateTimeOffset revokedAtUtc, string? replacedByToken = null)
        {
            RevokedAtUtc = revokedAtUtc;
            ReplacedByToken = replacedByToken;
            UpdatedAtUtc = revokedAtUtc;
        }
    }
}

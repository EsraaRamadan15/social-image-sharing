namespace Profiles.Domain.Entities
{
    public sealed class Profile : AuditableEntity<Guid>
    {
        private Profile()
        {
        }

        public Guid UserId { get; private set; }
        public string DisplayName { get; private set; } = string.Empty;
        public string? Bio { get; private set; }
        public string? AvatarUrl { get; private set; }
        public bool IsPrivate { get; private set; }

        public static Profile Create(
            Guid id,
            Guid userId,
            string displayName,
            DateTimeOffset createdAtUtc)
        {
            return new Profile
            {
                Id = id,
                UserId = userId,
                DisplayName = displayName,
                IsPrivate = false,
                CreatedAtUtc = createdAtUtc
            };
        }

        public void Update(
            string displayName,
            string? bio,
            string? avatarUrl,
            bool isPrivate,
            DateTimeOffset updatedAtUtc)
        {
            DisplayName = displayName;
            Bio = bio;
            AvatarUrl = avatarUrl;
            IsPrivate = isPrivate;
            UpdatedAtUtc = updatedAtUtc;
        }
    }
}

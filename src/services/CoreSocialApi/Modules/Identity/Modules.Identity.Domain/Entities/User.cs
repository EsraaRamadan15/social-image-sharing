using Domain.Common;
using Identity.Domain.Enums;

namespace Identity.Domain.Entities
{
    public sealed class User : AuditableEntity<Guid>
    {
        private User()
        {
        }
        public string UserName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; }

        public static User Create(
            Guid id,
            string userName,
            string email,
            string passwordHash,
            UserRole role,
            DateTimeOffset createdAtUtc)
        {
            return new User
            {
                Id = id,
                UserName = userName,
                Email = email,
                PasswordHash = passwordHash,
                Role = role,
                IsActive = true,
                CreatedAtUtc = createdAtUtc
            };
        }

        public void Deactivate(DateTimeOffset updatedAtUtc)
        {
            IsActive = false;
            UpdatedAtUtc = updatedAtUtc;
        }
    }
}

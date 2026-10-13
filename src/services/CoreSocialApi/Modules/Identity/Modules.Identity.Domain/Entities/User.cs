using Domain.Common;
using Identity.Domain.Enums;
using Identity.Domain.Events;

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
          string username,
          string email,
          string passwordHash,
          DateTimeOffset createdAtUtc)
        {
            var user = new User
            {
                Id = id,
                UserName = username,
                Email = email,
                PasswordHash = passwordHash,
                CreatedAtUtc = createdAtUtc
            };

            user.RaiseDomainEvent(
                new UserRegisteredDomainEvent(
                    user.Id,
                    user.UserName,
                    createdAtUtc));

            return user;
        }

        public void Deactivate(DateTimeOffset updatedAtUtc)
        {
            IsActive = false;
            UpdatedAtUtc = updatedAtUtc;
        }
    }
}

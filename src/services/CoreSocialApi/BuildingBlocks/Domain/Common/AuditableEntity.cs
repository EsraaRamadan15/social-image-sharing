namespace Domain.Common
{
    public abstract class AuditableEntity<TId> : Entity<TId>
    {
        public DateTimeOffset CreatedAtUtc { get; set; }
        public DateTimeOffset? UpdatedAtUtc { get; set; }
    }
}

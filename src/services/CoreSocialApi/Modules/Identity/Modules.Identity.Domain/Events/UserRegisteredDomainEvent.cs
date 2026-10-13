using Domain.Events;

namespace Identity.Domain.Events
{
    public sealed record UserRegisteredDomainEvent(
     Guid UserId,
     string Username,
     DateTimeOffset OccurredOnUtc)
     : IDomainEvent;
}

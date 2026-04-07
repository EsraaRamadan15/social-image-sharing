using Application.Abstractions;

namespace Infrastructure.Time
{
    public sealed class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}

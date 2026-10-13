namespace Infrastructure.Messaging.Outbox
{
    public sealed class OutboxMessage
    {
        public Guid Id { get; set; }

        public DateTimeOffset OccurredOnUtc { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Payload { get; set; } = string.Empty;

        public DateTimeOffset? ProcessedOnUtc { get; set; }

        public string? Error { get; set; }
    }
}

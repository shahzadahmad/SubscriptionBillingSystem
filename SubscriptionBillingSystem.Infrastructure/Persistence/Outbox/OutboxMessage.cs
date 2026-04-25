namespace SubscriptionBillingSystem.Infrastructure.Persistence.Outbox
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime OccurredOn { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public bool IsProcessed { get; set; }

        // 🔥 Retry support
        public int RetryCount { get; set; }
        public int MaxRetryCount { get; set; } = 5;

        public string? LastError { get; set; }

        public DateTime? NextRetryAt { get; set; }
    }
}
namespace SubscriptionBillingSystem.Application.Features.Invoices.Models
{
    public class GetInvoicesDto
    {
        public Guid InvoiceId { get; set; }
        public Guid SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

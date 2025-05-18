namespace E_Commerce.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; } = default!;
        public List<OrderItem> Items { get; set; } = [];
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public string StripeSessionId { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    public enum PaymentStatus
    {
        Pending,
        Paid,
    }
}

namespace E_Commerce.Services
{
    public class WebhookService(ApplicationDbContext context ) : IWebhookService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task MarkOrderAsPaidAsync(string stripeSessionId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.StripeSessionId == stripeSessionId);
            if (order is null)
            {
                Console.WriteLine($"Order not found for Session ID: {stripeSessionId}");
                throw new Exception("Order not found for the given Stripe session ID.");
            }

            Console.WriteLine($"Updating order: {order.Id} to Paid");
            order.PaymentStatus = PaymentStatus.Paid;
            order.PaidAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

        }
    }
}

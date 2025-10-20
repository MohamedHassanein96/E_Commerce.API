using Stripe;
using Stripe.Checkout;

namespace E_Commerce.Services
{
    public class WebhookService(ApplicationDbContext context, IInvoiceService invoiceService, ILogger<WebhookService> logger, IConfiguration configuration) : IWebhookService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IInvoiceService _invoiceService = invoiceService;
        private readonly ILogger<WebhookService> _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        public async Task MarkOrderAsPaidAsync(string stripeSessionId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _context.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(o => o.StripeSessionId == stripeSessionId);

                if (order is null)
                {
                    Console.WriteLine($"❌ Order not found for Session ID: {stripeSessionId}");
                    throw new Exception("Order not found for the given Stripe session ID.");
                }

                if (order.PaymentStatus == PaymentStatus.Paid)
                {
                    Console.WriteLine($"⚠️ Order {order.Id} already marked as Paid.");
                    return;
                }

                order.PaymentStatus = PaymentStatus.Paid;
                order.PaidAt = DateTime.UtcNow;

                foreach (var item in order.Items)
                {
                    var product = item.Product;

                    if (product is null)
                    {
                        Console.WriteLine($"⚠️ Product for order item {item.Id} not found.");
                        continue;
                    }

                    if (product.ReservedStock >= item.Quantity)
                    {
                        product.ReservedStock -= item.Quantity;
                    }

                    product.AvailableStock -= item.Quantity;
                    product.Version++;
                }

                _context.Update(order);
                await _context.SaveChangesAsync();

                var invoicePath = await _invoiceService.GenerateInvoiceAsync(order);
                order.InvoicePath = invoicePath;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "❌ Error while marking order as paid for Stripe session {StripeSessionId}", stripeSessionId);
                throw;
            }
        }


        public async Task HandleWebhookAsync(string json, string stripeSignature)
        {
            var endpointSecret = _configuration["Stripe:WebhookSecret"];

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, endpointSecret);

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    var sessionId = session.Id;
                    if (session == null)
                    {
                        Console.WriteLine("[Webhook] Session object is null.");
                        return;
                    }

                    Console.WriteLine($"[Webhook] Received session ID: {session.Id}");

                    var paymentIntentId = session.PaymentIntentId;
                    if (string.IsNullOrEmpty(paymentIntentId))
                    {
                        Console.WriteLine("[Webhook] PaymentIntent ID is null or empty.");
                        return;
                    }

                    var paymentIntentService = new PaymentIntentService();
                    var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId);

                    if (paymentIntent.Status == "succeeded")
                    {
                        Console.WriteLine("[Webhook] Payment succeeded.");
                        await MarkOrderAsPaidAsync(session.Id);
                    }
                    else
                    {
                        Console.WriteLine($"[Webhook] Payment not successful: {paymentIntent.Status}");
                    }
                }
                else
                {
                    Console.WriteLine($"[Webhook] Ignored event type: {stripeEvent.Type}");
                }
            }
            catch (StripeException e)
            {
                Console.WriteLine($"[Webhook] Stripe error: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Webhook] General error: {e.Message}");
                throw;
            }
        }
    }
}

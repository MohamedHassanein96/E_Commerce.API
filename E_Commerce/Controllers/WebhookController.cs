using Stripe;
using Stripe.Checkout;
namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController(IConfiguration configuration, IWebhookService webhookService) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IWebhookService _webhookService = webhookService;

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var endpointSecret = _configuration["Stripe:WebhookSecret"];
            string stripeSignature = Request.Headers["Stripe-Signature"];

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, endpointSecret);

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;

                    if (session != null)
                    {
                        Console.WriteLine($"[Webhook] Session ID received: {session.Id}");

                        var paymentIntentId = session.PaymentIntentId;
                        if (!string.IsNullOrEmpty(paymentIntentId))
                        {
                            var service = new PaymentIntentService();
                            var paymentIntent = await service.GetAsync(paymentIntentId);

                            if (paymentIntent.Status == "succeeded")
                            {
                                Console.WriteLine("payment is Done");
                                await _webhookService.MarkOrderAsPaidAsync(session.Id);
                            }
                            else
                            {
                                Console.WriteLine($"payment isn't successful: {paymentIntent.Status}");
                            }

                            return Ok(); 
                        }
                        else
                        {
                            Console.WriteLine(" PaymentIntent ID is null or empty.");
                            return BadRequest();
                        }
                    }
                    else
                    {
                        Console.WriteLine(" Session object is null.");
                        return BadRequest();
                    }
                }

                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine($"Stripe webhook error: {e.Message}");
                return BadRequest();
            }
            catch (Exception e)
            {
                Console.WriteLine($"General webhook error: {e.Message}");
                return StatusCode(500);
            }

        }
    }
}

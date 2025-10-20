namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IWebhookService _webhookService;
        private readonly IInvoiceService _invoiceService;

        public WebhookController( IWebhookService webhookService, IInvoiceService invoiceService)
        {
            _webhookService = webhookService;
            _invoiceService = invoiceService;
        }

        [HttpPost("")]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            try
            {
                await _webhookService.HandleWebhookAsync(json, stripeSignature!);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }

        }

        [HttpGet("invoices/{orderId}")]
        public async Task<IActionResult> GetInvoice(int orderId)
        {
            try
            {
                var fileBytes = await _invoiceService.GetInvoiceFileAsync(orderId);
                return File(fileBytes, "application/pdf", $"invoice_{orderId}.pdf");
            }
            catch (FileNotFoundException)
            {
                return NotFound("Invoice not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InvoiceController] Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
       
    }
}

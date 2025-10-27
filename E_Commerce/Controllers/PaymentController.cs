using E_Commerce.Extension;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IPaymentService _paymentService) : ControllerBase
    {
        [HttpPost("create-session")]
        public async Task<IActionResult> CreateCheckoutSession(CancellationToken cancellationToken)
        {
            var result = await _paymentService.CreateCheckoutSessionAsync(User.GetUserId()!,cancellationToken);
            return result.Match<IActionResult>(
                  success => Ok(success),
                  error => BadRequest(error.Message));
        }
    }
}

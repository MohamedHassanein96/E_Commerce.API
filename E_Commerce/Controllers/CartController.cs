using Microsoft.AspNetCore.Authorization;
using OneOf.Types;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController(ICartService cartService ) : ControllerBase
    {
        private readonly ICartService _cartService = cartService;
        
        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] AddToCartRequest request, CancellationToken cancellationToken)
        {
            var result = await _cartService.AddToCartAsync(request, cancellationToken);
            return result.Match<IActionResult>(
                  success => Ok(success),
                  error => BadRequest(error.Message));
                
        }

        [HttpGet("")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
           var result = await _cartService.GetCartDetailsAsync(cancellationToken);

            return result.Match<IActionResult>(
              success => Ok(),
              error => BadRequest($"Failed: {error.Message}"));


        }


        [HttpPut("decrement")]
        public async Task<IActionResult> Decrement([FromBody] DecrementRequest request, CancellationToken cancellationToken)
        {
            var result = await _cartService.DecrementAsync(request, cancellationToken);

            return result.Match<IActionResult>(
             success => Ok(),
              error => BadRequest($"Failed: {error.Message}"));
        }


        [HttpPut("increment")]
        public async Task<IActionResult> Increment([FromBody] IncrementRequest request, CancellationToken cancellationToken)
        {

            var result = await _cartService.IncrementAsync(request, cancellationToken);
            return result.Match<IActionResult>(
           success => Ok(),
           error => BadRequest($"Failed: {error.Message}"));
        }

        [HttpPut("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest request, CancellationToken cancellationToken)
        {
            var result = await _cartService.DeleteAsync(request, cancellationToken);
            return result.Match<IActionResult>(
            success => Ok(),
            error => BadRequest($"Failed: {error.Message}"));
        }

        [HttpPost("pay")]
        public async Task<IActionResult> Pay( CancellationToken cancellationToken)
        {
            var result = await _cartService.PayAsync(cancellationToken);
            return result.Match<IActionResult>(
                  success => Ok(),
                  error => BadRequest(error.Message));
        }
    }
}

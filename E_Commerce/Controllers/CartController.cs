using E_Commerce.CustomExceptions;
using Microsoft.AspNetCore.Authorization;

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
            var cartIsAdded = await _cartService.AddToCartAsync(request, cancellationToken);
            return cartIsAdded ? Ok() : BadRequest();
        }

        [HttpGet("")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {

            var cartResponse = await _cartService.GetCartDetailsAsync(cancellationToken);
            if (cartResponse is not null)
            {
                return Ok(cartResponse);
            }
            return NotFound("Cart not found or user not found.");
        }
        [HttpPut("decrement")]
        public async Task<IActionResult> Decrement([FromBody] DecrementRequest request, CancellationToken cancellationToken)
        {
            var isUpdated = await _cartService.DecrementAsync(request, cancellationToken);
            return isUpdated
                   ? NoContent() 
                   : NotFound(); 
        }
        [HttpPut("increment")]
        public async Task<IActionResult> Increment([FromBody] IncrementRequest request, CancellationToken cancellationToken)
        {

            var isUpdated = await _cartService.IncrementAsync(request, cancellationToken);
            return isUpdated
                  ? NoContent()       
                  : NotFound();      
        }

        [HttpPut("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest request, CancellationToken cancellationToken)
        {
            var isDeleted = await _cartService.DeleteAsync(request, cancellationToken);
            return isDeleted ? Ok("Deleted successfully.") : NotFound("Item not found in cart.");
        }

        [HttpPost("pay")]
        public async Task<IActionResult> Pay( CancellationToken cancellationToken)
        {
            try
            {
                var response = await _cartService.PayAsync(cancellationToken);
                if (response is null)
                    return BadRequest();

                return Ok(response);
            }
            catch (InsufficientStockException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

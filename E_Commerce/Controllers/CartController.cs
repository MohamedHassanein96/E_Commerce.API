using E_Commerce.Extension;
using Microsoft.AspNetCore.Authorization;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController(ICartService cartService) : ControllerBase
    {
        private readonly ICartService _cartService = cartService;

        [HttpPost("")]
        //[ServiceFilter(typeof(ValidateUserExistsFilter))]
        public async Task<IActionResult> Add([FromBody] AddToCartRequest request, CancellationToken cancellationToken)
        {
            var result = await _cartService.AddToCartAsync(User.GetUserId()!, request, cancellationToken);
            return result.Match<IActionResult>(
                  success => Ok(success),
                  error => BadRequest(error.Message));

        }

        [HttpGet("")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await _cartService.GetCartDetailsAsync(User.GetUserId()!, cancellationToken);

            return result.Match<IActionResult>(
              success => Ok(success),
              error => Unauthorized(error.Message));
        }


        [HttpPut("decrement")]
        public async Task<IActionResult> Decrement([FromBody] DecrementRequest request, CancellationToken cancellationToken)
        {
            var isDecremented = await _cartService.DecrementAsync(User.GetUserId()!, request, cancellationToken);

            if (isDecremented)
                return NoContent();
            return BadRequest();
        }


        [HttpPut("increment")]
        public async Task<IActionResult> Increment([FromBody] IncrementRequest request, CancellationToken cancellationToken)
        {

            var isIncremented = await _cartService.IncrementAsync(User.GetUserId()!, request, cancellationToken);
            if (isIncremented)
                return NoContent();
            return BadRequest();
        }

        [HttpPut("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest request, CancellationToken cancellationToken)
        {
            var isDeleted = await _cartService.DeleteAsync(User.GetUserId()!, request, cancellationToken);
            if (isDeleted)
                return NoContent();
            return BadRequest();
        }

       
    }
}

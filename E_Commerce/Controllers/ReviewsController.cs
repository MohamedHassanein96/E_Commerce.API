using E_Commerce.Contracts.Review;
using E_Commerce.Extension;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController( IReviewService _reviewService) : ControllerBase
{

    [HttpPost("")]
    [Authorize]
    public async Task<IActionResult> AddReview([FromBody] ReviewRequest request)
    {

        var result = await _reviewService.AddReviewAsync(User.GetUserId()!, request);
        return result.Match<IActionResult>(
           success => Ok(success),
           error => BadRequest(error.Message)

        );
    }
    [HttpGet("{productId}")]
    [Authorize]
    public async Task<IActionResult> GetReviewsByProduct([FromRoute] int productId)
    {

        var result = await _reviewService.GetReviewsByProductIdAsync(User.GetUserId()!, productId);
        return result.Match<IActionResult>(
           success => Ok(success),
           error => BadRequest(error.Message)

        );
    }

    [HttpGet("average/{productId}")]
    public async Task<IActionResult> GetAverageRating(int productId)
    {
        var avg = await _reviewService.GetAverageRatingAsync(User.GetUserId()!, productId);
        return Ok(new { productId, average = avg });
    }
}

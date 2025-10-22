using E_Commerce.Contracts.Review;
using E_Commerce.Extension;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController(IHuggingFaceService huggingFaceService, ApplicationDbContext context, IReviewService _reviewService, IHttpContextAccessor _httpContextAccessor) : ControllerBase
{
    private readonly IHuggingFaceService _huggingFaceService = huggingFaceService;
    private readonly ApplicationDbContext _context = context;

    //[HttpPost("add")]
    //public async Task<IActionResult> AddReview([FromBody] ReviewRequest request)
    //{


    //    var sentiment = await _huggingFaceService.AnalyzeSentimentAsync(request.UserReview);

    //    var review = new Review
    //    {
    //        UserReview = request.UserReview,
    //        ProductId = request.ProductId,
    //        Sentiment = sentiment,
    //        Stars = request.Stars
    //    };

    //    await _context.Reviews.AddAsync(review);
    //    await _context.SaveChangesAsync();


    //    var allRatings = await _context.Reviews
    //   .Where(r => r.ProductId == request.ProductId).ToListAsync();

    //    var avgRating = allRatings.Average(r => r.Stars);

    //    var product = await _context.Products.FindAsync(request.ProductId);
    //    product!.Rate = avgRating;
    //    //product.IsTopRated = avgRating >= 4.5; // review

    //    _context.Products.Update(product);
    //    await _context.SaveChangesAsync();


    //    return Ok(new { Message = "Review added successfully", Sentiment = sentiment });
    //}
    [HttpPost("")]
    [Authorize]
    public async Task<IActionResult> AddReview([FromBody] ReviewRequest request)
    {

        var result = await _reviewService.AddReviewAsync(User.GetUserId()!,request);
        return result.Match<IActionResult>(
           success => Ok(success),
           error => BadRequest(error.Message)

        );
    }
    [HttpGet("{productId}")]
    [Authorize]
    public async Task<IActionResult> GetReviewsByProduct([FromRoute] int productId)
    {

        var result = await _reviewService.GetReviewsByProductIdAsync(User.GetUserId()!,productId);
        return result.Match<IActionResult>(
           success => Ok(success),
           error => BadRequest(error.Message)

        );
    }

    [HttpGet("average/{productId}")]
    public async Task<IActionResult> GetAverageRating(int productId)
    {
        var avg = await _reviewService.GetAverageRatingAsync(User.GetUserId()!,productId);
        return Ok(new { productId, average = avg });
    }
}

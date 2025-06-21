using E_Commerce.Contracts.Review;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController(IHuggingFaceService huggingFaceService, ApplicationDbContext context) : ControllerBase
{
    private readonly IHuggingFaceService _huggingFaceService = huggingFaceService;
    private readonly ApplicationDbContext _context = context;

    [HttpPost("add")]
    public async Task<IActionResult> AddReview([FromBody] ReviewRequest request)
    {
      

        var sentiment = await _huggingFaceService.AnalyzeSentimentAsync(request.UserReview);

        var review = new Review
        {
            UserReview = request.UserReview,
            ProductId = request.ProductId,
            Sentiment = sentiment,
            Stars = request.Stars
        };

        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();


        var allRatings = await _context.Reviews
       .Where(r => r.ProductId == request.ProductId).ToListAsync();

        var avgRating = allRatings.Average(r => r.Stars);

        var product = await _context.Products.FindAsync(request.ProductId);
        product!.Rate = avgRating;
        product.IsTopRated = avgRating >= 4.5;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();


        return Ok(new { Message = "Review added successfully", Sentiment = sentiment });
    }

}

using E_Commerce.Contracts.Review;
using E_Commerce.Extension;
using OneOf;

namespace E_Commerce.Services
{
    public class ReviewService(UserManager<ApplicationUser> _userManager, ApplicationDbContext _context) : IReviewService
    {
        public async Task<OneOf<ReviewResponse, ErrorResponse>> AddReviewAsync(string userId, ReviewRequest request)
        {

            if (string.IsNullOrEmpty(userId))
                return new ErrorResponse("User Not Found");


            var review = request.Adapt<Review>();
            review.UserId = userId;
            _context.Reviews.Add(review);

            await _context.SaveChangesAsync();

            await UpdateProductAverageRateAsync( userId, request.ProductId);

            return (review.Adapt<ReviewResponse>());

        }



        public async Task<double> GetAverageRatingAsync(string userId, int productId)
        {
            var avg = await _context.Reviews
                .Where(r => r.ProductId == productId && r.Stars > 0)
                .AverageAsync(r => (double?)r.Stars) ?? 0; // review
            return Math.Round(avg, 2);
        }

        public async Task<OneOf<IEnumerable<ReviewResponse>, ErrorResponse>> GetReviewsByProductIdAsync(string userId, int productId)
        {
            if (string.IsNullOrEmpty(userId))
                return new ErrorResponse("User not authenticated");

            return await _context.Reviews
                                    .Where(r => r.ProductId == productId)
                                    .OrderByDescending(r => r.CreatedAt)
                                    .ProjectToType<ReviewResponse>()
                                    .AsNoTracking()
                                    .ToListAsync();

        }

        private async Task UpdateProductAverageRateAsync(string userId, int productId)
        {
            var avg = await GetAverageRatingAsync(userId, productId);
            var product = await _context.Products.FindAsync(productId);

            if (product is not null)
            {
                product.Rate = avg;
                await _context.SaveChangesAsync();
            }
        }
    }
}

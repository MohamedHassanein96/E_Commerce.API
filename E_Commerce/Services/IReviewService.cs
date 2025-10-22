using E_Commerce.Contracts.Review;
using OneOf;

namespace E_Commerce.Services
{
    public interface IReviewService
    {
        Task<OneOf<ReviewResponse, ErrorResponse>> AddReviewAsync(string userId,ReviewRequest request);
        Task<OneOf<IEnumerable<ReviewResponse>, ErrorResponse>> GetReviewsByProductIdAsync(string userId, int productId);
        Task<double> GetAverageRatingAsync(string userId,int productId);
    }
}

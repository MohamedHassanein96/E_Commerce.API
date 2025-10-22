namespace E_Commerce.Contracts.Review
{
    public record ReviewResponse(  int Id,  int ProductId, int Stars, string? UserReview, string? Sentiment, DateTime CreatedAt);
    
}

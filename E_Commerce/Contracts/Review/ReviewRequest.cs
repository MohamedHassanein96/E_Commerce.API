namespace E_Commerce.Contracts.Review
{
    public record ReviewRequest(string? Comment, int ProductId,int Stars);
    
}

namespace E_Commerce.Contracts.Cart
{
    public record CartResponse(IEnumerable<CartDetailsResponse> CartDetails , decimal TotalPrice);
    
    
}


namespace E_Commerce.Services
{
    public interface ICartService
    {
        Task<bool>  AddToCartAsync(AddToCartRequest request , CancellationToken cancellationToken=default);
        Task<CartResponse>  GetCartDetailsAsync(CancellationToken cancellationToken=default);
        Task<bool>  DecrementAsync(DecrementRequest request , CancellationToken cancellationToken=default);
        Task<bool>  IncrementAsync(IncrementRequest request , CancellationToken cancellationToken=default);
        Task<bool> DeleteAsync(DeleteRequest request , CancellationToken cancellationToken=default);
        Task<PayResponse> PayAsync ( CancellationToken cancellationToken=default);
    }
}

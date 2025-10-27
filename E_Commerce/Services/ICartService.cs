
using OneOf;

namespace E_Commerce.Services
{
    public interface ICartService
    {
        Task<OneOf<bool, ErrorResponse>>  AddToCartAsync(string userId,AddToCartRequest request , CancellationToken cancellationToken=default);
        Task<OneOf<CartResponse, ErrorResponse>>  GetCartDetailsAsync(string userId,CancellationToken cancellationToken=default);
        Task<bool>  DecrementAsync(string userId, DecrementRequest request , CancellationToken cancellationToken=default);
        Task<bool> IncrementAsync(string userId, IncrementRequest request , CancellationToken cancellationToken=default);
        Task<bool> DeleteAsync(string userId,DeleteRequest request , CancellationToken cancellationToken=default);
    }
}

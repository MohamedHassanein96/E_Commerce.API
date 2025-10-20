
using OneOf;

namespace E_Commerce.Services
{
    public interface ICartService
    {
        Task<OneOf<bool, ErrorResponse>>  AddToCartAsync(AddToCartRequest request , CancellationToken cancellationToken=default);
        Task<OneOf<CartResponse, ErrorResponse>>  GetCartDetailsAsync(CancellationToken cancellationToken=default);
        Task<OneOf<bool,ErrorResponse>>  DecrementAsync(DecrementRequest request , CancellationToken cancellationToken=default);
        Task<OneOf<bool,ErrorResponse>> IncrementAsync(IncrementRequest request , CancellationToken cancellationToken=default);
        Task<OneOf<bool, ErrorResponse>> DeleteAsync(DeleteRequest request , CancellationToken cancellationToken=default);
        Task<OneOf<PayResponse, ErrorResponse>> PayAsync(CancellationToken cancellationToken = default);
    }
}

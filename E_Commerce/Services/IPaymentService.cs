namespace E_Commerce.Services
{
    public interface IPaymentService
    {
        Task<OneOf<PayResponse, ErrorResponse>> CreateCheckoutSessionAsync(string userId, CancellationToken cancellationToken = default);

    }
}

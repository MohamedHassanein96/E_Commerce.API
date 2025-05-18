using E_Commerce.Contracts.Auth;

namespace E_Commerce.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    }
}

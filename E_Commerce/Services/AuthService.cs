using E_Commerce.Authentication;
using E_Commerce.Contracts.Auth;

namespace E_Commerce.Services
{
    public class AuthService( UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager ,IJwtProvider jwtProvider) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;

        public async Task<AuthResponse> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            if (await _userManager.FindByEmailAsync(email) is not { } user)
                return null!;

            var result = await _signInManager.PasswordSignInAsync(user, password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var (token, expiresIn) = _jwtProvider.GenerateToken(user);
                var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn);
                return(response);
            }
            return null!;
        }
    }
}

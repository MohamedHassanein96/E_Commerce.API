using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims; // ClaimTypes : .NET Identity Claims is used inside the ClaimPrincipal

namespace E_Commerce
{
    public class ValidateUserExistsFilter(UserManager<ApplicationUser> _userManager) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userId = context.HttpContext.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                    ?? context.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Unauthorized user." });
                return;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "User no longer exists." });
                return;
            }
            await next();
        }
    }
}

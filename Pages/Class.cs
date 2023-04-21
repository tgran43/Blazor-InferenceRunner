using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<CustomAuthenticationStateProvider> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public CustomAuthenticationStateProvider(SignInManager<IdentityUser> signInManager, ILogger<CustomAuthenticationStateProvider> logger, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _logger = logger;
        _userManager = userManager;
    }

    private IEnumerable<Claim> CreateClaims(IdentityUser user)
    {
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email)
    };

        return claims;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = _signInManager.Context.User;
        return new AuthenticationState(new ClaimsPrincipal(user));
    }

    public async Task SignInAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var result = await _signInManager.PasswordSignInAsync(user, password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var identity = new ClaimsIdentity(CreateClaims(user), "Identity.Application");
                var principal = new ClaimsPrincipal(identity);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
            }
        }
    }
}

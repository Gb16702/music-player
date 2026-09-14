using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Infrastructure.Identity;

namespace MusicPlayer.Infrastructure.Google
{
    internal sealed class GoogleLoginChallengeService : IGoogleLoginChallengeService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public GoogleLoginChallengeService(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public Task ChallengeAsync(HttpContext httpContext, string redirectUrl, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(
                GoogleDefaults.AuthenticationScheme,
                redirectUrl);

            return httpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
        }
    }
}

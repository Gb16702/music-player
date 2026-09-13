using Microsoft.AspNetCore.Identity;
using MusicPlayer.Application.Abstractions.Identity;

namespace MusicPlayer.Infrastructure.Identity
{
    internal sealed class AuthSignOutService : IAuthSignOutService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthSignOutService(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public Task SignOutAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _signInManager.SignOutAsync();
        }
    }
}

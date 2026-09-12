using Microsoft.AspNetCore.Identity;
using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Common;
using MusicPlayer.Application.Users.Login;

namespace MusicPlayer.Infrastructure.Identity
{
    internal sealed class AuthSignInService : IAuthSignInService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthSignInService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<Result<Guid>> SignInAsync(string email, string password, bool isPersistent, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Result<Guid>.Failure(LoginErrors.InvalidCredentials());
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

            if (!signInResult.Succeeded)
            {
                return Result<Guid>.Failure(LoginErrors.InvalidCredentials());
            }

            await _signInManager.SignInAsync(user, isPersistent);

            return Result<Guid>.Success(user.Id);
        }
    }
}

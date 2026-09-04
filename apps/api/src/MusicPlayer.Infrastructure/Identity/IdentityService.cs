using Microsoft.AspNetCore.Identity;
using MusicPlayer.Application.Abstractions.Identity;

namespace MusicPlayer.Infrastructure.Identity
{
    internal sealed class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Guid> CreateUserAsync(string email, string password, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserName = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException("User creation failed");
            }

            return user.Id;
        }
    }
}

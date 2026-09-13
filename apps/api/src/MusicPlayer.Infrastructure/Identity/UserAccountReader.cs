using Microsoft.AspNetCore.Identity;
using MusicPlayer.Application.Abstractions.Identity;

namespace MusicPlayer.Infrastructure.Identity
{
    internal sealed class UserAccountReader : IUserAccountReader
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserAccountReader(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string?> GetEmailAsync(Guid userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByIdAsync(userId.ToString());

            return user?.Email;
        }
    }
}

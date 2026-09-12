using Microsoft.AspNetCore.Identity;
using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Common;

namespace MusicPlayer.Infrastructure.Identity
{
    internal sealed class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<Guid>> CreateUserAsync(string email, string password, CancellationToken cancellationToken)
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
                return Result<Guid>.Failure(IdentityErrorMapper.Map(result.Errors));
            }

            return Result<Guid>.Success(user.Id);
        }
    }
}

using MusicPlayer.Application.Abstractions.Identity;

namespace MusicPlayer.Application.Users.Logout
{
    public sealed class LogoutUserHandler : ILogoutUserHandler
    {
        private readonly IAuthSignOutService _authSignOutService;

        public LogoutUserHandler(IAuthSignOutService authSignOutService)
        {
            _authSignOutService = authSignOutService;
        }

        public Task HandleAsync(CancellationToken cancellationToken)
        {
            return _authSignOutService.SignOutAsync(cancellationToken);
        }
    }
}

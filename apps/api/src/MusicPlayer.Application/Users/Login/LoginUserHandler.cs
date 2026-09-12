using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.Login
{
    public sealed class LoginUserHandler : ILoginUserHandler
    {
        private readonly IAuthSignInService _authSignInService;

        public LoginUserHandler(IAuthSignInService authSignInService)
        {
            _authSignInService = authSignInService;
        }

        public Task<Result<Guid>> HandleAsync(LoginUserCommand command, CancellationToken cancellationToken)
        {
            return _authSignInService.SignInAsync(command.Email, command.Password, command.IsPersistent, cancellationToken);
        }
    }
}

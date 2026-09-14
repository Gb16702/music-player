using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.SignInWithExternalProvider
{
    public sealed class SignInWithExternalProviderHandler : ISignInWithExternalProviderHandler
    {
        private readonly IExternalAuthSignInService _externalAuthSignInService;

        public SignInWithExternalProviderHandler(IExternalAuthSignInService externalAuthSignInService)
        {
            _externalAuthSignInService = externalAuthSignInService;
        }

        public Task<Result<Guid>> HandleAsync(SignInWithExternalProviderCommand command, CancellationToken cancellationToken)
        {
            return _externalAuthSignInService.SignInAsync(
                command.Email,
                command.LoginProvider,
                command.ProviderKey,
                cancellationToken);
        }
    }
}

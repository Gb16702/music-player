using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Common;
using MusicPlayer.Application.Users.RequestMagicLink;

namespace MusicPlayer.Application.Users.VerifyMagicLink
{
    public sealed class VerifyMagicLinkHandler : IVerifyMagicLinkHandler
    {
        private readonly IMagicLinkTokenService _magicLinkTokenService;
        private readonly IMagicLinkSignInService _magicLinkSignInService;

        public VerifyMagicLinkHandler(IMagicLinkTokenService magicLinkTokenService, IMagicLinkSignInService magicLinkSignInService)
        {
            _magicLinkTokenService = magicLinkTokenService;
            _magicLinkSignInService = magicLinkSignInService;
        }

        public async Task<Result<Guid>> HandleAsync(VerifyMagicLinkCommand command, CancellationToken cancellationToken)
        {
            var tokenValidationResult = _magicLinkTokenService.ValidateToken(command.Token);

            if (!tokenValidationResult.IsSuccess)
            {
                return Result<Guid>.Failure(MapTokenError(tokenValidationResult.Error!));
            }

            return await _magicLinkSignInService.SignInAsync(tokenValidationResult.Value!, cancellationToken);
        }

        private static Error MapTokenError(Error error)
        {
            return error.Code switch
            {
                MagicLinkTokenErrors.ExpiredTokenCode => VerifyMagicLinkErrors.ExpiredToken(),
                _ => VerifyMagicLinkErrors.InvalidToken(),
            };
        }
    }
}

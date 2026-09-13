using MusicPlayer.Application.Abstractions.Email;
using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.RequestMagicLink
{
    public sealed class RequestMagicLinkHandler : IRequestMagicLinkHandler
    {
        private readonly IMagicLinkTokenService _magicLinkTokenService;
        private readonly IMagicLinkUrlBuilder _magicLinkUrlBuilder;
        private readonly IEmailSender _emailSender;

        public RequestMagicLinkHandler(
            IMagicLinkTokenService magicLinkTokenService,
            IMagicLinkUrlBuilder magicLinkUrlBuilder,
            IEmailSender emailSender)
        {
            _magicLinkTokenService = magicLinkTokenService;
            _magicLinkUrlBuilder = magicLinkUrlBuilder;
            _emailSender = emailSender;
        }

        public async Task<Result<bool>> HandleAsync(RequestMagicLinkCommand command, CancellationToken cancellationToken)
        {
            var normalizedEmail = command.Email.Trim();

            var token = _magicLinkTokenService.GenerateToken(normalizedEmail);
            var magicLinkUrl = _magicLinkUrlBuilder.BuildVerifyUrl(token);

            await _emailSender.SendMagicLinkAsync(normalizedEmail, magicLinkUrl, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}

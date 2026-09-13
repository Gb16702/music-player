using Microsoft.Extensions.Options;
using MusicPlayer.Application.Abstractions.Email;
using Resend;

namespace MusicPlayer.Infrastructure.Email
{
    internal sealed class ResendEmailSender : IEmailSender
    {
        private readonly IResend _resend;
        private readonly EmailOptions _emailOptions;
        private readonly MagicLinkOptions _magicLinkOptions;

        public ResendEmailSender(IResend resend, IOptions<EmailOptions> emailOptions, IOptions<MagicLinkOptions> magicLinkOptions)
        {
            _resend = resend;
            _emailOptions = emailOptions.Value;
            _magicLinkOptions = magicLinkOptions.Value;
        }

        public async Task SendMagicLinkAsync(string toEmail, string magicLinkUrl, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var (subject, htmlBody) = MagicLinkEmailContent.Create(magicLinkUrl, _magicLinkOptions.TokenLifetimeMinutes);

            var message = new EmailMessage
            {
                From = $"{_emailOptions.FromName} <{_emailOptions.FromAddress}>",
                To = [toEmail],
                Subject = subject,
                HtmlBody = htmlBody
            };

            await _resend.EmailSendAsync(message, cancellationToken);
        }
    }
}

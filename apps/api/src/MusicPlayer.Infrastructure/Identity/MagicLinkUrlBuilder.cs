using Microsoft.Extensions.Options;
using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Infrastructure.Email;

namespace MusicPlayer.Infrastructure.Identity
{
    internal sealed class MagicLinkUrlBuilder : IMagicLinkUrlBuilder
    {
        private readonly MagicLinkOptions _options;

        public MagicLinkUrlBuilder(IOptions<MagicLinkOptions> options)
        {
            _options = options.Value;
        }

        public string BuildVerifyUrl(string token)
        {
            var baseUrl = _options.WebAppBaseUrl.TrimEnd('/');
            var encodedToken = Uri.EscapeDataString(token);

            return $"{baseUrl}/auth/magic-link?token={encodedToken}";
        }
    }
}

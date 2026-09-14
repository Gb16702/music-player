using Microsoft.Extensions.Options;
using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Infrastructure.Email;

namespace MusicPlayer.Infrastructure.Identity
{
    internal sealed class WebAppRedirectBuilder : IWebAppRedirectBuilder
    {
        private readonly MagicLinkOptions _options;

        public WebAppRedirectBuilder(IOptions<MagicLinkOptions> options)
        {
            _options = options.Value;
        }

        public string BuildAuthCallbackSuccessUrl()
        {
            return $"{_options.WebAppBaseUrl.TrimEnd('/')}/auth/callback";
        }

        public string BuildAuthCallbackErrorUrl(string errorCode, string? detail = null)
        {
            var encodedError = Uri.EscapeDataString(errorCode);
            var url = $"{_options.WebAppBaseUrl.TrimEnd('/')}/auth/callback?error={encodedError}";

            if (!string.IsNullOrWhiteSpace(detail))
            {
                url += $"&detail={Uri.EscapeDataString(detail)}";
            }

            return url;
        }
    }
}

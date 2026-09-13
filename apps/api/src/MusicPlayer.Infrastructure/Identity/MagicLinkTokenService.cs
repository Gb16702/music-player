using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Common;
using MusicPlayer.Application.Users.RequestMagicLink;
using MusicPlayer.Infrastructure.Email;

namespace MusicPlayer.Infrastructure.Identity
{
    internal sealed class MagicLinkTokenService : IMagicLinkTokenService
    {
        private const string ProtectorPurpose = "MusicPlayer.MagicLink.v1";

        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly MagicLinkOptions _options;

        public MagicLinkTokenService(IDataProtectionProvider dataProtectionProvider, IOptions<MagicLinkOptions> options)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _options = options.Value;
        }

        public string GenerateToken(string email)
        {
            var normalizedEmail = NormalizeEmail(email);
            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.TokenLifetimeMinutes);
            var payload = $"{normalizedEmail}|{expiresAt.UtcTicks}";

            return _dataProtectionProvider.CreateProtector(ProtectorPurpose).Protect(payload);
        }

        public Result<string> ValidateToken(string token)
        {
            try
            {
                var payload = _dataProtectionProvider.CreateProtector(ProtectorPurpose).Unprotect(token);
                var separatorIndex = payload.IndexOf('|', StringComparison.Ordinal);

                if (separatorIndex <= 0 || separatorIndex == payload.Length - 1)
                {
                    return Result<string>.Failure(MagicLinkTokenErrors.InvalidToken());
                }

                var email = payload[..separatorIndex];
                var expiresAtTicks = payload[(separatorIndex + 1)..];

                if (!long.TryParse(expiresAtTicks, out var ticks))
                {
                    return Result<string>.Failure(MagicLinkTokenErrors.InvalidToken());
                }

                if (new DateTimeOffset(ticks, TimeSpan.Zero) < DateTimeOffset.UtcNow)
                {
                    return Result<string>.Failure(MagicLinkTokenErrors.ExpiredToken());
                }

                return Result<string>.Success(email);
            }
            catch (Exception)
            {
                return Result<string>.Failure(MagicLinkTokenErrors.InvalidToken());
            }
        }

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }
    }
}

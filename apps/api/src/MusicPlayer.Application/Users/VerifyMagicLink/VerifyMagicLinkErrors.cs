using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.VerifyMagicLink
{
    public static class VerifyMagicLinkErrors
    {
        public const string InvalidTokenCode = "invalid_magic_link_token";

        public const string ExpiredTokenCode = "expired_magic_link_token";

        public static Error InvalidToken() => new(InvalidTokenCode, "Magic link token is invalid.");

        public static Error ExpiredToken() => new(ExpiredTokenCode, "Magic link token has expired.");
    }
}

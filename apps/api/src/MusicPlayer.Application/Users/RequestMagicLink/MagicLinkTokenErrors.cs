using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.RequestMagicLink
{
    public static class MagicLinkTokenErrors
    {
        public const string InvalidTokenCode = "invalid_magic_link_token";

        public const string ExpiredTokenCode = "expired_magic_link_token";

        public static Error InvalidToken() => new(InvalidTokenCode, "Magic link token is invalid.");

        public static Error ExpiredToken() => new(ExpiredTokenCode, "Magic link token has expired.");
    }
}

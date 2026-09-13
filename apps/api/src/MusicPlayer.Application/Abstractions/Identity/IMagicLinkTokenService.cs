using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Abstractions.Identity
{
    public interface IMagicLinkTokenService
    {
        string GenerateToken(string email);

        Result<string> ValidateToken(string token);
    }
}

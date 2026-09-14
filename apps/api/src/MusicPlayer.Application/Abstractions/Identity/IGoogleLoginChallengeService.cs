using Microsoft.AspNetCore.Http;

namespace MusicPlayer.Application.Abstractions.Identity
{
    public interface IGoogleLoginChallengeService
    {
        Task ChallengeAsync(HttpContext httpContext, string redirectUrl, CancellationToken cancellationToken = default);
    }
}

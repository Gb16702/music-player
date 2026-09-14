using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Common;
using MusicPlayer.Application.Users.SignInWithExternalProvider;
using MusicPlayer.Infrastructure.Identity;

namespace MusicPlayer.Infrastructure.Google
{
    internal sealed class GoogleSignInCallbackService : IGoogleSignInCallbackService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ISignInWithExternalProviderHandler _signInWithExternalProviderHandler;
        private readonly ILogger<GoogleSignInCallbackService> _logger;

        public GoogleSignInCallbackService(
            IHttpContextAccessor httpContextAccessor,
            SignInManager<ApplicationUser> signInManager,
            ISignInWithExternalProviderHandler signInWithExternalProviderHandler,
            ILogger<GoogleSignInCallbackService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _signInWithExternalProviderHandler = signInWithExternalProviderHandler;
            _logger = logger;
        }

        public async Task<Result<Guid>> CompleteSignInAsync(CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is null)
            {
                return Result<Guid>.Failure(ExternalAuthErrors.SignInFailed("HTTP context is unavailable."));
            }

            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (externalLoginInfo is null)
            {
                var externalAuth = await httpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

                _logger.LogWarning(
                    "Google callback missing external login info. ExternalSchemeSucceeded={Succeeded}, Failure={Failure}",
                    externalAuth.Succeeded,
                    externalAuth.Failure?.Message);

                return Result<Guid>.Failure(ExternalAuthErrors.SignInFailed("External login information is unavailable."));
            }

            var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email)
                ?? externalLoginInfo.Principal.FindFirstValue("email");

            if (string.IsNullOrWhiteSpace(email))
            {
                await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                return Result<Guid>.Failure(ExternalAuthErrors.EmailRequired());
            }

            var result = await _signInWithExternalProviderHandler.HandleAsync(
                new SignInWithExternalProviderCommand(
                    email.Trim(),
                    externalLoginInfo.LoginProvider,
                    externalLoginInfo.ProviderKey),
                cancellationToken);

            await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return result;
        }
    }
}

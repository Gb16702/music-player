using Microsoft.Extensions.DependencyInjection;
using MusicPlayer.Application.Users.CompleteOnboarding;
using MusicPlayer.Application.Users.GetCurrentUser;
using MusicPlayer.Application.Users.Login;
using MusicPlayer.Application.Users.Logout;
using MusicPlayer.Application.Users.Register;
using MusicPlayer.Application.Users.RequestMagicLink;
using MusicPlayer.Application.Users.SignInWithExternalProvider;
using MusicPlayer.Application.Users.VerifyMagicLink;

namespace MusicPlayer.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IRegisterUserHandler, RegisterUserHandler>();
            services.AddScoped<ILoginUserHandler, LoginUserHandler>();
            services.AddScoped<ILogoutUserHandler, LogoutUserHandler>();
            services.AddScoped<IGetCurrentUserHandler, GetCurrentUserHandler>();
            services.AddScoped<ICompleteOnboardingHandler, CompleteOnboardingHandler>();
            services.AddScoped<IRequestMagicLinkHandler, RequestMagicLinkHandler>();
            services.AddScoped<IVerifyMagicLinkHandler, VerifyMagicLinkHandler>();
            services.AddScoped<ISignInWithExternalProviderHandler, SignInWithExternalProviderHandler>();

            return services;
        }
    }
}

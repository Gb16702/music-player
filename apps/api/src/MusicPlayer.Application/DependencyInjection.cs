using Microsoft.Extensions.DependencyInjection;
using MusicPlayer.Application.Users.GetCurrentUser;
using MusicPlayer.Application.Users.Login;
using MusicPlayer.Application.Users.Logout;
using MusicPlayer.Application.Users.Register;

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

            return services;
        }
    }
}

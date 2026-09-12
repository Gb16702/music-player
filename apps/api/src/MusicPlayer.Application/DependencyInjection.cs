using Microsoft.Extensions.DependencyInjection;
using MusicPlayer.Application.Users.Login;
using MusicPlayer.Application.Users.Register;

namespace MusicPlayer.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IRegisterUserHandler, RegisterUserHandler>();
            services.AddScoped<ILoginUserHandler, LoginUserHandler>();

            return services;
        }
    }
}

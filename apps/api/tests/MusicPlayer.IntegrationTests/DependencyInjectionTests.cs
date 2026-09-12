using Microsoft.Extensions.DependencyInjection;
using MusicPlayer.Application.Users.Register;

namespace MusicPlayer.IntegrationTests;

public sealed class DependencyInjectionTests(MusicPlayerApiFactory application)
    : IClassFixture<MusicPlayerApiFactory>
{
    [Fact]
    public void RegisterUserHandlerIsRegistered()
    {
        using var scope = application.Services.CreateScope();

        var handler = scope.ServiceProvider.GetService<IRegisterUserHandler>();

        Assert.NotNull(handler);
        Assert.IsType<RegisterUserHandler>(handler);
    }
}

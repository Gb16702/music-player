using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MusicPlayer.Application.Abstractions.Email;

namespace MusicPlayer.IntegrationTests;

public sealed class MusicPlayerApiFactory : WebApplicationFactory<Program>
{
    public CapturingEmailSender EmailSender { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        IntegrationTestConfiguration.ConfigureTestHost(builder);

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IEmailSender>();
            services.AddSingleton(EmailSender);
            services.AddSingleton<IEmailSender>(provider => provider.GetRequiredService<CapturingEmailSender>());
        });
    }
}

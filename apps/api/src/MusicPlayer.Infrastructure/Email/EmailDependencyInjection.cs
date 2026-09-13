using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicPlayer.Application.Abstractions.Email;
using Resend;

namespace MusicPlayer.Infrastructure.Email
{
    internal static class EmailDependencyInjection
    {
        public static IServiceCollection AddEmail(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<EmailOptions>()
                .Bind(configuration.GetSection(EmailOptions.SectionName))
                .Validate(options => !string.IsNullOrWhiteSpace(options.FromAddress), "Email:FromAddress is required")
                .ValidateOnStart();

            services.AddOptions<SmtpOptions>()
                .Bind(configuration.GetSection(SmtpOptions.SectionName));

            services.AddOptions<ResendOptions>()
                .Bind(configuration.GetSection(ResendOptions.SectionName));

            services.AddOptions<MagicLinkOptions>()
                .Bind(configuration.GetSection(MagicLinkOptions.SectionName))
                .Validate(options => !string.IsNullOrWhiteSpace(options.WebAppBaseUrl), "MagicLink:WebAppBaseUrl is required")
                .Validate(options => options.TokenLifetimeMinutes > 0, "MagicLink:TokenLifetimeMinutes must be greater than zero")
                .ValidateOnStart();

            var provider = configuration["Email:Provider"] ?? "Log";

            switch (provider.Trim().ToLowerInvariant())
            {
                case "resend":
                    services.AddOptions<ResendClientOptions>().Configure(options =>
                    {
                        options.ApiToken = configuration[$"{ResendOptions.SectionName}:ApiKey"] ?? string.Empty;
                    });

                    services.AddHttpClient<IResend, ResendClient>();
                    services.AddScoped<IEmailSender, ResendEmailSender>();
                    break;

                case "smtp":
                    services.AddScoped<IEmailSender, SmtpEmailSender>();
                    break;

                default:
                    services.AddScoped<IEmailSender, LogEmailSender>();
                    break;
            }

            return services;
        }
    }
}

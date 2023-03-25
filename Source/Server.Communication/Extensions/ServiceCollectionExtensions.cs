using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TModLoaderMaintainer.Infrastructure.Server.Communication.Configuration;
using TModLoaderMaintainer.Infrastructure.Server.Communication.Contracts.Services;
using TModLoaderMaintainer.Infrastructure.Server.Communication.Factories;
using TModLoaderMaintainer.Infrastructure.Server.Communication.Services;
using TModLoaderMaintainer.Utilities.DependencyInjection.Common.Extensions.Extensions;

namespace TModLoaderMaintainer.Infrastructure.Server.Communication.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureServerCommunicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var serverConnectionSettings = configuration.GetSection("ServerConnection");
            services.AddOptionsWithValidation<ServerConnectionSettings>(serverConnectionSettings);

            services.AddScoped<ConnectionInfoFactory>();
            services.AddScoped(x =>
            {
                var factory = x.GetRequiredService<ConnectionInfoFactory>();
                return factory.ConnectionInfo;
            });

            services.AddScoped<ISftpConnectionService, SftpConnectionService>();
            services.AddScoped<ISshConnectionService, SshConnectionService>();

            return services;
        }
    }
}

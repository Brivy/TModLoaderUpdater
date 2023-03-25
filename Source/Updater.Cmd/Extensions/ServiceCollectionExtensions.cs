using Microsoft.Extensions.DependencyInjection;
using TModLoaderMaintainer.Clients.Updater.Cmd.Contracts;
using TModLoaderMaintainer.Clients.Updater.Cmd.Services;

namespace TModLoaderMaintainer.Clients.Updater.Cmd.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureLocalServices(this IServiceCollection services)
        {
            services.AddScoped<IServerUpdateService, ServerUpdateService>();

            return services;
        }
    }
}

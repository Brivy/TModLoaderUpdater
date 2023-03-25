using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TModLoaderMaintainer.Application.Updater.Business.Configuration;
using TModLoaderMaintainer.Application.Updater.Business.Contracts.Services;
using TModLoaderMaintainer.Application.Updater.Business.Services;
using TModLoaderMaintainer.Utilities.DependencyInjection.Common.Extensions.Extensions;

namespace TModLoaderMaintainer.Application.Updater.Business.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureUpdaterBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            var manualModConfigurationSection = configuration.GetSection("ManualModConfiguration");
            services.AddOptionsWithValidation<ManualModConfigurationSettings>(manualModConfigurationSection);

            var fileDirectoryConfigurationSection = configuration.GetSection("FileDirectoryConfiguration");
            services.AddOptionsWithValidation<FileDirectoryConfigurationSettings>(fileDirectoryConfigurationSection);

            services.AddScoped<IModFinderService, ModFinderService>();
            services.AddScoped<IProjectFileService, ProjectFileService>();
            services.AddScoped<IServerModUpdaterService, ServerModUpdaterService>();
            services.AddScoped<IServerFilesUpdaterService, ServerFilesUpdaterService>();
            services.AddScoped<ISystemFileService, SystemFileService>();

            return services;
        }
    }
}

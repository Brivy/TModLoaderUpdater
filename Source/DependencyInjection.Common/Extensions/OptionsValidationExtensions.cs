using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TModLoaderMaintainer.Utilities.DependencyInjection.Common.Extensions.Extensions
{
    public static class OptionsValidationExtensions
    {
        public static IServiceCollection AddOptionsWithValidation<TOptions>(this IServiceCollection services, IConfiguration config) where TOptions : class
        {
            services.ConfigureAndValidate<TOptions>(config.Bind);
            return services;
        }
    }
}
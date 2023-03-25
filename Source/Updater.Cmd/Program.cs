using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TModLoaderMaintainer.Application.Updater.Business.Extensions;
using TModLoaderMaintainer.Clients.Updater.Cmd.Contracts;
using TModLoaderMaintainer.Clients.Updater.Cmd.Extensions;
using TModLoaderMaintainer.Infrastructure.Server.Communication.Extensions;

namespace TModLoaderMaintainer.Clients.Updater.Cmd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
               .Build();


            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.ConfigureLocalServices();
                    services.ConfigureServerCommunicationServices(configuration);
                    services.ConfigureUpdaterBusinessServices(configuration);
                })
                .Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var serviceProvider = serviceScope.ServiceProvider;
                var serverUpdateService = serviceProvider.GetRequiredService<IServerUpdateService>();

                serverUpdateService.Update();
            }

            Console.ReadLine();
        }
    }
}
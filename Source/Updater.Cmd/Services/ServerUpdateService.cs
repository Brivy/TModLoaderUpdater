using Microsoft.Extensions.Logging;
using TModLoaderMaintainer.Clients.Updater.Cmd.Exceptions;
using TModLoaderMaintainer.Application.Updater.Business.Contracts.Services;
using TModLoaderMaintainer.Clients.Updater.Cmd.Contracts;

namespace TModLoaderMaintainer.Clients.Updater.Cmd.Services
{
    public class ServerUpdateService : IServerUpdateService
    {
        private readonly IServerFilesUpdaterService _serverFilesUpdaterService;
        private readonly IServerModUpdaterService _serverModUpdaterService;
        private readonly ISystemFileService _systemFileService;
        private readonly ILogger<ServerUpdateService> _logger;

        public ServerUpdateService(
            IServerFilesUpdaterService serverFilesUpdaterService,
            IServerModUpdaterService serverModUpdaterService,
            ISystemFileService systemFileService,
            ILogger<ServerUpdateService> logger)
        {
            _serverFilesUpdaterService = serverFilesUpdaterService;
            _serverModUpdaterService = serverModUpdaterService;
            _systemFileService = systemFileService;
            _logger = logger;
        }

        public void Update()
        {
            try
            {
                var mods = _systemFileService.RetrieveModsFromWorkshop();
                if (!mods.Any())
                {
                    throw new ModNotFoundException();
                }

                _serverModUpdaterService.Update(mods);
                _serverFilesUpdaterService.Update();

                _logger.LogInformation("Done updating server. Press any key to exit the updater...");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unhandled exception has occurred while updating the server");
            }
        }
    }
}

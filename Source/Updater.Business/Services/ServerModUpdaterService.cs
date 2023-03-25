using Microsoft.Extensions.Logging;
using System.Text.Json;
using TModLoaderMaintainer.Application.Updater.Business.Builders;
using TModLoaderMaintainer.Application.Updater.Business.Contracts.Services;
using TModLoaderMaintainer.Infrastructure.Server.Communication.Contracts.Services;

namespace TModLoaderMaintainer.Application.Updater.Business.Services
{
    public class ServerModUpdaterService : IServerModUpdaterService
    {
        private readonly ISftpConnectionService _tModSftpService;
        private readonly ISshConnectionService _tModSshService;
        private readonly ILogger<ServerModUpdaterService> _logger;

        public ServerModUpdaterService(
            ISftpConnectionService tModSftpService,
            ISshConnectionService tModSshService,
            ILogger<ServerModUpdaterService> logger)
        {
            _tModSftpService = tModSftpService;
            _tModSshService = tModSshService;
            _logger = logger;
        }

        public void Update(List<FileInfo> mods)
        {
            if (!mods.Any())
            {
                _logger.LogWarning("No mods found to be uploaded. Please check if the steam workshop location is correct in the configuration");
                return;
            }

            var sftpServerAction = new SftpServerActionBuilder()
                .AddLoggingAction("Uploading mods")
                .AddChangeDirectoryAction(".local/share/Terraria/tModLoader/Mods")
                .AddUploadFilesAction(mods)
                .Build();

            _tModSftpService.Execute(sftpServerAction);

            var enabledMods = JsonSerializer.Serialize(mods.Select(x => x.Name.Remove(x.Name.Length - 5)).ToList());
            var sshServerAction = new SshServerActionBuilder()
                .AddLoggingAction("Enable mods on server")
                .AddRunCommandAction($"cd .local/share/Terraria/tModLoader/Mods && echo '{enabledMods}' > enabled.json")
                .Build();

            _tModSshService.Execute(sshServerAction);
        }
    }
}

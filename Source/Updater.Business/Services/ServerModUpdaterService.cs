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

        public ServerModUpdaterService(
            ISftpConnectionService tModSftpService,
            ISshConnectionService tModSshService)
        {
            _tModSftpService = tModSftpService;
            _tModSshService = tModSshService;
        }

        public void Update(List<FileInfo> mods)
        {
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

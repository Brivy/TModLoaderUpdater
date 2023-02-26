using TModLoaderMaintainer.Application.Updater.Business.Builders;
using TModLoaderMaintainer.Application.Updater.Business.Contracts.Services;
using TModLoaderMaintainer.Infrastructure.Server.Communication.Contracts.Services;
using TModLoaderMaintainer.Models.ProjectFiles.Constants;

namespace TModLoaderMaintainer.Application.Updater.Business.Services
{
    public class ServerFilesUpdaterService : IServerFilesUpdaterService
    {
        private readonly ISftpConnectionService _tModSftpService;
        private readonly ISshConnectionService _tModSshService;
        private readonly IProjectFileService _projectFileService;

        public ServerFilesUpdaterService(
            ISftpConnectionService tModSftpService,
            ISshConnectionService tModSshService,
            IProjectFileService projectFileService)
        {
            _tModSftpService = tModSftpService;
            _tModSshService = tModSshService;
            _projectFileService = projectFileService;
        }

        public void Update()
        {
            var steamStartFile = _projectFileService.OpenProjectFile(ProjectFileNames.StartTModWithoutSteamFile);
            var startupFile = _projectFileService.OpenProjectFile(ProjectFileNames.StartServerFile);
            var serverConfigFile = _projectFileService.OpenProjectFile(ProjectFileNames.ServerConfigFile);

            if (steamStartFile == null || startupFile == null || serverConfigFile == null)
            {
                throw new Exception();
            }

            var serverFiles = new List<FileInfo> { steamStartFile, startupFile, serverConfigFile };
            var sftpServerAction = new SftpServerActionBuilder()
                .AddLoggingAction("Uploading server files")
                .AddUploadFilesAction(serverFiles)
                .Build();

            _tModSftpService.Execute(sftpServerAction);

            var sshServerAction = new SshServerActionBuilder()
                .AddLoggingAction("Updating server files")
                .AddRunCommandAction("wget https://github.com/tModLoader/tModLoader/releases/latest/download/tModLoader.zip")
                .AddRunCommandAction("rm -rf ~/tmod/ && unzip -o tModLoader.zip -d ~/tmod/ && rm tModLoader.zip")
                .AddRunCommandAction("chmod u+x startup.sh && chmod u+x start-tModLoaderServerWithoutSteam.sh")
                .AddRunCommandAction("chmod u+x startup.sh && chmod u+x start-tModLoaderServerWithoutSteam.sh")
                .AddRunCommandAction("mv start-tModLoaderServerWithoutSteam.sh ~/tmod/ && mv serverconfig.txt ~/tmod/")
                .Build();

            _tModSshService.Execute(sshServerAction);
        }
    }
}

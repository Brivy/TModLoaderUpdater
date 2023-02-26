using Microsoft.Extensions.Logging;
using Renci.SshNet;
using TModLoaderMaintainer.Infrastructure.Server.Communication.Contracts.Services;
using TModLoaderMaintainer.Models.DataTransferObjects;
using TModLoaderMaintainer.Models.Enums;

namespace TModLoaderMaintainer.Infrastructure.Server.Communication.Services
{
    public class SftpConnectionService : ISftpConnectionService
    {
        private readonly ConnectionInfo _connectionInfo;
        private readonly ILogger<SftpConnectionService> _logger;

        private double _currentFileSize;
        private int _lastPercentageShown;

        public SftpConnectionService(
            ConnectionInfo connectionInfo,
            ILogger<SftpConnectionService> logger)
        {
            _connectionInfo = connectionInfo;
            _logger = logger;
        }

        public void Execute(ServerActionDto<SftpActionDto> serverAction)
        {
            try
            {
                using var sftpClient = new SftpClient(_connectionInfo);
                sftpClient.Connect();
                _logger.LogInformation("Connected to the server via SFTP");

                while (serverAction.Actions.Count > 0)
                {
                    var action = serverAction.Actions.Dequeue();
                    switch (action.Action)
                    {
                        case SftpAction.Logging:
                            _logger.LogInformation("{command}", action.Command);
                            break;
                        case SftpAction.ChangeDirectory:
                            sftpClient.ChangeDirectory(action.Command);
                            break;
                        case SftpAction.UploadFile:
                            if (action.FileInfo == null)
                            {
                                _logger.LogError("File that needs to be uploaded not found, aborting...");
                                throw new NullReferenceException();
                            }

                            _logger.LogInformation("Copying: {fullName}", action.FileInfo.FullName);
                            using (var fileStream = action.FileInfo.OpenRead())
                            {
                                _currentFileSize = fileStream.Length;
                                sftpClient.UploadFile(fileStream, action.FileInfo.Name, true, UploadProgressCallback);
                            }
                            break;
                        default:
                            break;
                    }
                }

                sftpClient.Disconnect();
                _logger.LogInformation("Disconnected the SFTP connection");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An exception has occurred while executing SFTP commands. Remaining calls are aborted");
                serverAction.ClearActions();
            }
        }

        private void UploadProgressCallback(ulong uploaded)
        {
            var progress = (int)(uploaded / _currentFileSize * 100);
            if (progress == 100)
            {
                _lastPercentageShown = 0;
                _logger.LogInformation("Upload progress: 100%");
                _logger.LogInformation("Upload complete");
            }
            else if (progress % 20 == 0 && _lastPercentageShown != progress)
            {
                _lastPercentageShown = progress;
                _logger.LogInformation("Upload progress: {progress}%", progress);
            }
        }
    }
}

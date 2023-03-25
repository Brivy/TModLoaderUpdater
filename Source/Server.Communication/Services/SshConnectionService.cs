using Microsoft.Extensions.Logging;
using Renci.SshNet;
using TModLoaderMaintainer.Infrastructure.Server.Communication.Contracts.Services;
using TModLoaderMaintainer.Models.DataTransferObjects;
using TModLoaderMaintainer.Models.Enums;

namespace TModLoaderMaintainer.Infrastructure.Server.Communication.Services
{
    public class SshConnectionService : ISshConnectionService
    {
        private readonly ConnectionInfo _connectionInfo;
        private readonly ILogger<SshConnectionService> _logger;

        public SshConnectionService(
            ConnectionInfo connectionInfo,
            ILogger<SshConnectionService> logger)
        {
            _connectionInfo = connectionInfo;
            _logger = logger;
        }

        public void Execute(ServerActionDto<SshActionDto> serverAction)
        {
            try
            {
                using var sshClient = new SshClient(_connectionInfo);
                sshClient.Connect();
                _logger.LogInformation("Connected to the server via SSH");

                while (serverAction.Actions.Count > 0)
                {
                    var action = serverAction.Actions.Dequeue();
                    switch (action.Action)
                    {
                        case SshAction.Logging:
                            _logger.LogInformation("{command}", action.Command);
                            break;
                        case SshAction.RunCommand:
                            sshClient.RunCommand(action.Command);
                            break;
                        default:
                            break;
                    }
                }

                sshClient.Disconnect();
                _logger.LogInformation("Disconnected the SSH connection");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An exception has occurred while executing SSH commands. Remaining calls are aborted");
                serverAction.ClearActions();
            }
        }
    }
}

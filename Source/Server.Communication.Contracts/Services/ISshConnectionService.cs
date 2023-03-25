using TModLoaderMaintainer.Models.DataTransferObjects;

namespace TModLoaderMaintainer.Infrastructure.Server.Communication.Contracts.Services
{
    public interface ISshConnectionService
    {
        void Execute(ServerActionDto<SshActionDto> serverAction);
    }
}
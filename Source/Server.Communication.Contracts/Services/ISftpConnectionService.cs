using TModLoaderMaintainer.Models.DataTransferObjects;

namespace TModLoaderMaintainer.Infrastructure.Server.Communication.Contracts.Services
{
    public interface ISftpConnectionService
    {
        void Execute(ServerActionDto<SftpActionDto> serverAction);
    }
}
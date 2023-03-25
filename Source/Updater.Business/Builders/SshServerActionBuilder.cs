using TModLoaderMaintainer.Models.DataTransferObjects;
using TModLoaderMaintainer.Models.Enums;

namespace TModLoaderMaintainer.Application.Updater.Business.Builders
{
    public class SshServerActionBuilder
    {
        private ServerActionDto<SshActionDto> _serverAction = new();

        public ServerActionDto<SshActionDto> Build() => _serverAction;

        public SshServerActionBuilder AddLoggingAction(string command)
        {
            _serverAction.Actions.Enqueue(new SshActionDto(SshAction.Logging, command));
            return this;
        }

        public SshServerActionBuilder AddRunCommandAction(string command)
        {
            _serverAction.Actions.Enqueue(new SshActionDto(SshAction.RunCommand, command));
            return this;
        }
    }
}

using TModLoaderMaintainer.Models.Enums;

namespace TModLoaderMaintainer.Models.DataTransferObjects
{
    public record class SshActionDto
    {
        public SshAction Action { get; }
        public string Command { get; }

        public SshActionDto(SshAction action, string command)
        {
            Action = action;
            Command = command;
        }
    }
}

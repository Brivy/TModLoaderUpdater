using TModLoaderMaintainer.Models.Enums;

namespace TModLoaderMaintainer.Models.DataTransferObjects
{
    public record class SftpActionDto
    {
        public SftpAction Action { get; }
        public string? Command { get; }
        public FileInfo? FileInfo { get; }

        public SftpActionDto(SftpAction action, string command)
        {
            Action = action;
            Command = command;
        }

        public SftpActionDto(SftpAction action, FileInfo fileInfo)
        {
            Action = action;
            FileInfo = fileInfo;
        }
    }
}

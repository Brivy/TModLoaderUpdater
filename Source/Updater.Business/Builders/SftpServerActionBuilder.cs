using TModLoaderMaintainer.Models.DataTransferObjects;
using TModLoaderMaintainer.Models.Enums;

namespace TModLoaderMaintainer.Application.Updater.Business.Builders
{
    public class SftpServerActionBuilder
    {
        private ServerActionDto<SftpActionDto> _serverAction = new();

        public ServerActionDto<SftpActionDto> Build() => _serverAction;

        public SftpServerActionBuilder AddLoggingAction(string command)
        {
            _serverAction.Actions.Enqueue(new SftpActionDto(SftpAction.Logging, command));
            return this;
        }

        public SftpServerActionBuilder AddChangeDirectoryAction(string command)
        {
            _serverAction.Actions.Enqueue(new SftpActionDto(SftpAction.ChangeDirectory, command));
            return this;
        }

        public SftpServerActionBuilder AddUploadFileAction(FileInfo fileInfo)
        {
            _serverAction.Actions.Enqueue(new SftpActionDto(SftpAction.UploadFile, fileInfo));
            return this;
        }

        public SftpServerActionBuilder AddUploadFilesAction(IEnumerable<FileInfo> fileInfos)
        {
            foreach (var fileInfo in fileInfos)
                _serverAction.Actions.Enqueue(new SftpActionDto(SftpAction.UploadFile, fileInfo));
            return this;
        }
    }
}

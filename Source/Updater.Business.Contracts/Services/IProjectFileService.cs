namespace TModLoaderMaintainer.Application.Updater.Business.Contracts.Services
{
    public interface IProjectFileService
    {
        FileInfo OpenProjectFile(string path);
    }
}
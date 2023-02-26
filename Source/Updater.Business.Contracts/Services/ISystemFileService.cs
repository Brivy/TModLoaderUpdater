namespace TModLoaderMaintainer.Application.Updater.Business.Contracts.Services
{
    public interface ISystemFileService
    {
        List<FileInfo> RetrieveModsFromWorkshop();
    }
}
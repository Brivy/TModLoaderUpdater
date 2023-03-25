namespace TModLoaderMaintainer.Application.Updater.Business.Contracts.Services
{
    public interface IServerModUpdaterService
    {
        void Update(List<FileInfo> mods);
    }
}
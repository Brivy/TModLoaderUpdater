namespace TModLoaderMaintainer.Application.Updater.Business.Contracts.Services
{
    public interface IModFinderService
    {
        bool IsDisabledMod(string workshopName);
        bool IsVersionSpecificMod(string workshopName);
        DirectoryInfo[]? GetModDirectory(DirectoryInfo modDirectory, int year, int lastMonth);
        DirectoryInfo[]? GetVersionSpecificModDirectory(DirectoryInfo modDirectory, string workshopName);
    }
}
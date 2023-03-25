using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TModLoaderMaintainer.Application.Updater.Business.Configuration;
using TModLoaderMaintainer.Application.Updater.Business.Contracts.Services;

namespace TModLoaderMaintainer.Application.Updater.Business.Services
{
    public class SystemFileService : ISystemFileService
    {
        private readonly IModFinderService _modFinderService;
        private readonly FileDirectoryConfigurationSettings _fileDirectoryConfigurationSettings;
        private readonly ILogger<SystemFileService> _logger;

        public SystemFileService(
            IModFinderService modFinderService,
            IOptions<FileDirectoryConfigurationSettings> fileDirectoryConfigurationSettings,
            ILogger<SystemFileService> logger)
        {
            _modFinderService = modFinderService;
            _fileDirectoryConfigurationSettings = fileDirectoryConfigurationSettings.Value;
            _logger = logger;
        }

        public List<FileInfo> RetrieveModsFromWorkshop()
        {
            var currentYear = DateTime.Now.Year;
            var lastMonth = DateTime.Now.Month - 1;
            if (lastMonth == 0)
            {
                // In case the current month is Januari, the last month is December
                currentYear--;
                lastMonth = 12;
            }

            var mods = new List<FileInfo>();
            var workshopDirectory = new DirectoryInfo(_fileDirectoryConfigurationSettings.SteamWorkshopLocation);
            foreach (var modDirectory in workshopDirectory.EnumerateDirectories())
            {
                if (_modFinderService.IsDisabledMod(modDirectory.Name))
                {
                    _logger.LogWarning("Mod {modName} found in the disabled mod list and will not be uploaded", modDirectory.Name);
                    continue;
                }

                var currentMod = _modFinderService.IsVersionSpecificMod(modDirectory.Name)
                    ? _modFinderService.GetVersionSpecificModDirectory(modDirectory, modDirectory.Name)
                    : _modFinderService.GetModDirectory(modDirectory, currentYear, lastMonth);

                if (currentMod == null)
                {
                    _logger.LogWarning("Mod {modName} was not found or has an build that is too old (< 12 months). " +
                        "Please specify the exact version in configuration if you want to use a mod with a build older than 12 months", modDirectory.Name);
                    continue;
                }

                var mod = currentMod.Single().GetFiles($"*.{_fileDirectoryConfigurationSettings.ModFileExtension}");
                if (mod.Length != 1)
                {
                    _logger.LogWarning("Mod {modName} with folder version {folderVersion} does not have a .{fileExtension} file available", modDirectory.Name, currentMod.Single().Name, _fileDirectoryConfigurationSettings.ModFileExtension);
                    continue;
                }

                mods.Add(mod.First());
            }

            return mods;
        }


    }
}

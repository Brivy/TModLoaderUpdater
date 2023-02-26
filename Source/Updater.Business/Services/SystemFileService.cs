using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TModLoaderMaintainer.Application.Updater.Business.Configuration;
using TModLoaderMaintainer.Application.Updater.Business.Contracts.Services;

namespace TModLoaderMaintainer.Application.Updater.Business.Services
{
    public class SystemFileService : ISystemFileService
    {
        private readonly IProjectFileService _projectFileService;
        private readonly FileDirectoryConfigurationSettings _fileDirectoryConfigurationSettings;
        private readonly ManualModConfigurationSettings _manualModConfigurationSettings;
        private readonly ILogger<SystemFileService> _logger;

        public SystemFileService(
            IProjectFileService projectFileService,
            IOptions<FileDirectoryConfigurationSettings> fileDirectoryConfigurationSettings,
            IOptions<ManualModConfigurationSettings> manualModConfigurationSettings,
            ILogger<SystemFileService> logger)
        {
            _projectFileService = projectFileService;
            _fileDirectoryConfigurationSettings = fileDirectoryConfigurationSettings.Value;
            _manualModConfigurationSettings = manualModConfigurationSettings.Value;
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
                if (_manualModConfigurationSettings.DisabledMods.Any(x => x.WorkshopName == modDirectory.Name))
                {
                    _logger.LogWarning($"Mod {modDirectory.Name} found in the disabled mod list and will not be uploaded");
                    continue;
                }

                var versionSpecificMod = _manualModConfigurationSettings.VersionSpecificMods.SingleOrDefault(x => x.WorkshopName == modDirectory.Name);
                var currentMod = versionSpecificMod != null ? GetVersionSpecificModDirectory(modDirectory, versionSpecificMod) : GetModDirectory(modDirectory, currentYear, lastMonth);
                if (currentMod == null)
                {
                    _logger.LogWarning($"Mod {modDirectory.Name} was not found or has an build that is too old. Please specify the exact version in 'version-specific-mods.json' if you want to use a mod with a build older than 12 months");
                    continue;
                }

                var mod = currentMod.Single().GetFiles("*.tmod");
                if (mod.Length != 1)
                {
                    _logger.LogError($"Mod {modDirectory.Name} with folder version {currentMod.Single().Name} does not have a .tmod file available");
                    continue;
                }

                mods.Add(mod.First());
            }

            return mods;
        }

        private DirectoryInfo[]? GetVersionSpecificModDirectory(DirectoryInfo modDirectory, VersionSpecificMod versionSpecificMod)
        {
            _logger.LogInformation($"Mod {versionSpecificMod.Name} found in the version specific mod list so only version {versionSpecificMod.Year}.{versionSpecificMod.Month} will be uploaded");
            var currentMod = modDirectory.GetDirectories(GetSearchPattern(versionSpecificMod.Year, versionSpecificMod.Month));
            return currentMod?.Any() == true ? currentMod : null;
        }

        private DirectoryInfo[]? GetModDirectory(DirectoryInfo modDirectory, int year, int month)
        {
            var currentMod = modDirectory.GetDirectories(GetSearchPattern(year, month));
            if (currentMod.Length == 1)
            {
                return currentMod;
            }

            // If not found for last month, search for previous months
            for (var i = 1; i < 13; i++)
            {
                var previousMonth = month - i;
                if (previousMonth == 0) year--;
                if (previousMonth <= 0) previousMonth = 12 + month - i;

                currentMod = modDirectory.GetDirectories(GetSearchPattern(year, previousMonth));
                if (currentMod.Length == 1)
                {
                    return currentMod;
                }
            }

            return null;
        }

        private static string GetSearchPattern(int year, int month) =>
            $"*{year}.{month}";


    }
}

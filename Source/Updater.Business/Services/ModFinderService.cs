using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TModLoaderMaintainer.Application.Updater.Business.Configuration;
using TModLoaderMaintainer.Application.Updater.Business.Contracts.Services;

namespace TModLoaderMaintainer.Application.Updater.Business.Services
{
    public class ModFinderService : IModFinderService
    {
        private readonly ManualModConfigurationSettings _manualModConfigurationSettings;
        private readonly ILogger<ModFinderService> _logger;

        public ModFinderService(
            IOptions<ManualModConfigurationSettings> manualModConfigurationSettings,
            ILogger<ModFinderService> logger)
        {
            _manualModConfigurationSettings = manualModConfigurationSettings.Value;
            _logger = logger;
        }

        public DirectoryInfo[]? GetVersionSpecificModDirectory(DirectoryInfo modDirectory, string workshopName)
        {
            var mod = _manualModConfigurationSettings.VersionSpecificMods.Single(x => x.WorkshopName == workshopName);
            _logger.LogInformation("Mod {modName} found in the version specific mod list so only version {version} will be uploaded", mod.WorkshopName, $"{mod.Year}.{mod.Month}");

            var currentMod = modDirectory.GetDirectories(GetSearchPattern(mod.Year, mod.Month));
            return currentMod?.Any() == true ? currentMod : null;
        }

        public DirectoryInfo[]? GetModDirectory(DirectoryInfo modDirectory, int year, int lastMonth)
        {
            var currentMod = modDirectory.GetDirectories(GetSearchPattern(year, lastMonth));
            if (currentMod.Length == 1) return currentMod;

            // If not found for last month, search for previous months
            for (var i = 1; i < 13; i++)
            {
                var previousMonth = lastMonth - i;
                if (previousMonth == 0) year--;
                if (previousMonth <= 0) previousMonth = 12 + lastMonth - i;

                currentMod = modDirectory.GetDirectories(GetSearchPattern(year, previousMonth));
                if (currentMod.Length == 1) return currentMod;
            }

            return null;
        }

        public bool IsDisabledMod(string workshopName) => _manualModConfigurationSettings.DisabledMods.Any(x => x.WorkshopName == workshopName);

        public bool IsVersionSpecificMod(string workshopName) => _manualModConfigurationSettings.VersionSpecificMods.Any(x => x.WorkshopName == workshopName);

        private static string GetSearchPattern(int year, int month) => $"*{year}.{month}";
    }
}

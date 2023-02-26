using System.ComponentModel.DataAnnotations;

namespace TModLoaderMaintainer.Application.Updater.Business.Configuration
{
    public class ManualModConfigurationSettings
    {
        public List<VersionSpecificMod> VersionSpecificMods { get; set; }
        public List<DisabledMod> DisabledMods { get; set; }

        public ManualModConfigurationSettings()
        {
            VersionSpecificMods = new List<VersionSpecificMod>();
            DisabledMods = new List<DisabledMod>();
        }
    }

    public class VersionSpecificMod
    {
        [Required]
        public string WorkshopName { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public int Year { get; set; }

        [Required]
        public int Month { get; set; }
    }

    public class DisabledMod
    {
        [Required]
        public string WorkshopName { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;
    }
}

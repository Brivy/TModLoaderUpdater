﻿using System.ComponentModel.DataAnnotations;

namespace TModLoaderMaintainer.Application.Updater.Business.Configuration
{
    public class FileDirectoryConfigurationSettings
    {
        [Required]
        public string SteamWorkshopLocation { get; set; } = null!;

        [Required]
        public string ModFileExtension { get; set; } = null!;
    }
}

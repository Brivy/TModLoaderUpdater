namespace TModLoaderUpdater.Models
{
    public record VersionSpecificMod : Mod
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public string Version()
        {
            return $"{Year}.{Month}";
        }
    }
}

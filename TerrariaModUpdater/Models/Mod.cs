namespace TModLoaderUpdater.Models
{
    public abstract record Mod
    {
        public string WorkshopName { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}

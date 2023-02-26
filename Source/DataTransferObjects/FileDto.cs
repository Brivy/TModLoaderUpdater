namespace TModLoaderMaintainer.Models.DataTransferObjects
{
    public record class FileDto
    {
        public string Name { get; }
        public string FullName { get; }
        public Stream Data { get; }

        public FileDto(string name, string fullName, Stream data)
        {
            Name = name;
            FullName = fullName;
            Data = data;
        }
    }
}

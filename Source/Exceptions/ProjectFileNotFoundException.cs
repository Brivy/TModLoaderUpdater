namespace TModLoaderMaintainer.Models.Exceptions
{
    public class ProjectFileNotFoundException : Exception
    {
        public ProjectFileNotFoundException()
        {
        }

        public ProjectFileNotFoundException(string message)
            : base(message)
        {
        }

        public ProjectFileNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

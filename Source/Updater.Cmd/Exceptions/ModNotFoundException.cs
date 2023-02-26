namespace TModLoaderMaintainer.Clients.Updater.Cmd.Exceptions
{
    public class ModNotFoundException : Exception
    {
        public ModNotFoundException()
        {
        }

        public ModNotFoundException(string message)
            : base(message)
        {
        }

        public ModNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

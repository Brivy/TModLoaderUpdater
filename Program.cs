using System.Text.Json;
using Renci.SshNet;

namespace TModLoaderUpdater
{
    public class Program
    {
        public static void Main()
        {
            var mods = RetrieveModsFromWorkshop();
            var conn = SetupConnectionToServer();
            PushModsToServer(conn, mods);
            PushEnabledModsJsonToServer(conn, mods);
        }

        private static List<FileInfo> RetrieveModsFromWorkshop()
        {
            var lastMonth = DateTime.Now.Month - 1;
            var workshopLocation = @"C:\Program Files (x86)\Steam\steamapps\workshop\content\1281930";
            var workshopDirectory = new DirectoryInfo(workshopLocation);

            var mods = new List<FileInfo>();
            foreach (var modDirectory in workshopDirectory.EnumerateDirectories())
            {
                var currentMod = modDirectory.GetDirectories(GetSearchPattern(lastMonth));
                if (currentMod.Length != 1)
                {
                    currentMod = GetOlderModDirectory(modDirectory, lastMonth);
                    if (currentMod?.Any() != true)
                    {
                        continue;
                    }
                }

                var mod = currentMod.First().GetFiles("*.tmod");
                if (mod.Length != 1)
                {
                    continue;
                }

                mods.Add(mod.First());
            }

            return mods;
        }

        private static DirectoryInfo[]? GetOlderModDirectory(DirectoryInfo modDirectory, int month)
        {
            for (var i = 1; i < 13; i++)
            {
                var previousMonth = month - i;
                if (previousMonth == 0)
                {
                    previousMonth = 12 + month - i;
                }

                var currentMod = modDirectory.GetDirectories(GetSearchPattern(previousMonth));
                if (currentMod.Length == 1)
                {
                    return currentMod;
                }
            }

            return null;
        }

        private static string GetSearchPattern(int month) => 
            $"*.{month}";

        private static ConnectionInfo SetupConnectionToServer()
        {
            Console.Write("Enter server user name: ");
            var userName = Console.ReadLine();
            Console.Write("Enter server host: ");
            var host = Console.ReadLine();
            Console.Write("Enter SSH port: ");
            var port = Console.ReadLine();

            if (userName == null || host == null || port == null)
            {
                Console.WriteLine("User name, host and SSH port should have a value before continuing...");
                throw new ArgumentNullException();
            }

            if (!int.TryParse(port, out var parsedPort))
            {
                Console.WriteLine("Port number should be convertible to an integer...");
                throw new ArgumentException();
            }

            var pk = new PrivateKeyFile("C:\\Users\\Dx2br\\.ssh\\TerrariaServerKey_pem");
            var keyFiles = new[] { pk };
            var methods = new List<AuthenticationMethod> { new PrivateKeyAuthenticationMethod(userName, keyFiles) };
            return new ConnectionInfo(host, parsedPort, userName, methods.ToArray());
        }

        private static void PushModsToServer(ConnectionInfo conn, List<FileInfo> mods)
        {
            using var sftp = new SftpClient(conn);
            sftp.Connect();
            sftp.ChangeDirectory(".local/share/Terraria/tModLoader/Mods");

            foreach (var mod in mods)
            {
                Console.WriteLine($"Copying: {mod.FullName}");
                using var modStream = mod.OpenRead();
                sftp.UploadFile(modStream, mod.Name, true);
            }

            sftp.Disconnect();
        }

        private static void PushEnabledModsJsonToServer(ConnectionInfo conn, List<FileInfo> mods)
        {
            Console.WriteLine("Enabling all mods...");
            var enabledMods = JsonSerializer.Serialize(mods.Select(x => x.Name.Remove(x.Name.Length - 5)).ToList());
            using var sshClient = new SshClient(conn);
            sshClient.Connect();
            sshClient.RunCommand($"cd .local/share/Terraria/tModLoader/Mods && echo '{enabledMods}' > enabled.json");
            sshClient.Disconnect();
            Console.WriteLine("All mods enabled...");
        }
    }
}
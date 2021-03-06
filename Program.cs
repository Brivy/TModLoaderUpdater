using System.Reflection;
using System.Text;
using System.Text.Json;
using Renci.SshNet;

namespace TModLoaderUpdater
{
    public class Program
    {
        private static string? _userName;
        private static string? _host;
        private static string? _port;
        private static string? _privateKeyLocation;

        private static double _currentFileSize;
        private static int _lastPercentageShown;

        public static void Main(string[] args)
        {
            try
            {
                ParseArguments(args);
                var mods = RetrieveModsFromWorkshop();
                var conn = SetupConnectionToServer();
                PushModsToServer(conn, mods);
                UpdateServerFiles(conn);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Finished updating the server...");
            Console.ReadLine();
        }

        private static void ParseArguments(IReadOnlyList<string> args)
        {
            if (args.Count == 0)
            {
                return;
            }

            for (var i = 0; i < args.Count - 1; i++)
            {
                switch (args[i])
                {
                    case "-u":
                        _userName = args[i + 1];
                        break;
                    case "-h":
                        _host = args[i + 1];
                        break;
                    case "-p":
                        _port = args[i + 1];
                        break;
                    case "-l":
                        _privateKeyLocation = args[i + 1];
                        break;
                }
            }
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
            if (string.IsNullOrWhiteSpace(_userName))
            {
                Console.Write("Enter server user name: ");
                _userName = Console.ReadLine();
            }

            if (string.IsNullOrWhiteSpace(_host))
            {
                Console.Write("Enter server host: ");
                _host = Console.ReadLine();
            }

            if (string.IsNullOrWhiteSpace(_port))
            {
                Console.Write("Enter SSH port: ");
                _port = Console.ReadLine();
            }

            if (string.IsNullOrWhiteSpace(_privateKeyLocation))
            {
                Console.Write("Enter private key location: ");
                _privateKeyLocation = Console.ReadLine();
            }

            if (string.IsNullOrWhiteSpace(_userName) ||
                string.IsNullOrWhiteSpace(_host) ||
                string.IsNullOrWhiteSpace(_port) ||
                string.IsNullOrWhiteSpace(_privateKeyLocation))
            {
                Console.WriteLine("User name, host, SSH port and private key location should have a value before continuing");
                throw new ArgumentNullException();
            }

            if (!int.TryParse(_port, out var parsedPort))
            {
                Console.WriteLine("Port number should be convertible to an integer");
                throw new ArgumentException();
            }

            Console.WriteLine($"Setting up server connection with host address {_host}, port {_port} and user '{_userName}'");
            var pk = new PrivateKeyFile(_privateKeyLocation);
            var keyFiles = new[] { pk };
            var methods = new List<AuthenticationMethod> { new PrivateKeyAuthenticationMethod(_userName, keyFiles) };
            return new ConnectionInfo(_host, parsedPort, _userName, methods.ToArray());
        }

        private static void PushModsToServer(ConnectionInfo conn, List<FileInfo> mods)
        {
            using var sftp = new SftpClient(conn);
            sftp.Connect();
            sftp.ChangeDirectory(".local/share/Terraria/tModLoader/Mods");

            Console.WriteLine("SFTP connection for copying mods to the server succeeded");
            foreach (var mod in mods)
            {
                Console.WriteLine($"Copying: {mod.FullName}");
                using var modStream = mod.OpenRead();
                _currentFileSize = modStream.Length;
                sftp.UploadFile(modStream, mod.Name, true, UploadProgressCallback);
            }

            sftp.Disconnect();
            Console.WriteLine("SFTP connection with server closed");

            var enabledMods = JsonSerializer.Serialize(mods.Select(x => x.Name.Remove(x.Name.Length - 5)).ToList());
            using var sshClient = new SshClient(conn);
            sshClient.Connect();
            Console.WriteLine("SSH connection for enabling mods succeeded");
            sshClient.RunCommand($"cd .local/share/Terraria/tModLoader/Mods && echo '{enabledMods}' > enabled.json");
            Console.WriteLine("Enabled all mods on the server");
            sshClient.Disconnect();
            Console.WriteLine("SSH connection with server closed");
        }

        private static void UpdateServerFiles(ConnectionInfo conn)
        {
            var steamStartFile = OpenProjectFile("Resources/start-tModLoaderServerWithoutSteam.sh");
            var startupFile = OpenProjectFile("Resources/startup.sh");
            var serverConfigFile = OpenProjectFile("Resources/serverconfig.txt");
            var serverFiles = new List<FileInfo> { steamStartFile, startupFile, serverConfigFile };

            using var sftp = new SftpClient(conn);
            sftp.Connect();
            Console.WriteLine("SFTP connection for sending server files");
            foreach (var serverFile in serverFiles)
            {
                Console.WriteLine($"Copying: {serverFile.FullName}");
                using var fileStream = serverFile.OpenRead();
                _currentFileSize = fileStream.Length;
                sftp.UploadFile(fileStream, serverFile.Name, true, UploadProgressCallback);
            }

            sftp.Disconnect();
            Console.WriteLine("SFTP connection with server closed");

            using var sshClient = new SshClient(conn);
            sshClient.Connect();
            Console.WriteLine("SSH connection for updating the server files");
            sshClient.RunCommand("wget https://github.com/tModLoader/tModLoader/releases/latest/download/tModLoader.zip");
            sshClient.RunCommand("rm -rf ~/tmod/ && unzip -o tModLoader.zip -d ~/tmod/ && rm tModLoader.zip");
            sshClient.RunCommand("chmod u+x startup.sh && chmod u+x start-tModLoaderServerWithoutSteam.sh");
            sshClient.RunCommand("mv start-tModLoaderServerWithoutSteam.sh ~/tmod/ && mv serverconfig.txt ~/tmod/");
            sshClient.Disconnect();
            Console.WriteLine("SSH connection with server closed");
        }

        private static FileInfo OpenProjectFile(string path)
        {
            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyLocation == null)
            {
                throw new Exception();
            }

            var combinedPath = Path.Combine(assemblyLocation, path);
            return new FileInfo(combinedPath);
        }

        private static void UploadProgressCallback(ulong uploaded)
        {
            var progress = (int)(uploaded / _currentFileSize * 100);
            if (progress == 100)
            {
                _lastPercentageShown = 0;
                Console.WriteLine("Upload progress: 100%");
                Console.WriteLine("Upload complete");
            }
            else if (progress % 25 == 0 && _lastPercentageShown != progress)
            {
                _lastPercentageShown = progress;
                Console.WriteLine("Upload progress: " + progress + "%");
            }
        }
    }
}
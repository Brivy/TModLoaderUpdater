using Microsoft.Extensions.Options;
using Renci.SshNet;
using TModLoaderMaintainer.Infrastructure.Server.Communication.Configuration;

namespace TModLoaderMaintainer.Infrastructure.Server.Communication.Factories
{
    public class ConnectionInfoFactory
    {
        private readonly ServerConnectionSettings _serverConnectionSettings;

        public ConnectionInfo ConnectionInfo { get; }

        public ConnectionInfoFactory(IOptions<ServerConnectionSettings> options)
        {
            _serverConnectionSettings = options.Value;
            ConnectionInfo = Create();
        }

        public ConnectionInfo Create()
        {
            var pk = new PrivateKeyFile(_serverConnectionSettings.PrivateKeyLocation);
            var keyFiles = new[] { pk };
            var methods = new List<AuthenticationMethod> { new PrivateKeyAuthenticationMethod(_serverConnectionSettings.Username, keyFiles) };
            return new ConnectionInfo(_serverConnectionSettings.Host, _serverConnectionSettings.Port, _serverConnectionSettings.Username, methods.ToArray());
        }
    }
}

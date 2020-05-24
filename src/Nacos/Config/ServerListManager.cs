namespace Nacos.Config
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class ServerListManager
    {
        public const string FIXED_NAME = "fixed";

        private string _name;
        private string _namespace = "";
        private string _tenant = "";

        private readonly List<string> _serverUrls;
        private string _currentServerAddr;

        public ServerListManager(NacosOptions options)
        {
            _serverUrls = new List<string>();
            var serverAddresses = options.ServerAddresses;
            var @namespace = options.Namespace;

            if (serverAddresses != null && serverAddresses.Any())
            {
                foreach (var item in serverAddresses)
                {
                    // here only trust the input server addresses of user
                    _serverUrls.Add(item.TrimEnd('/'));
                }

                if (string.IsNullOrWhiteSpace(@namespace))
                {
                    _name = $"{FIXED_NAME}-{GetFixedNameSuffix(_serverUrls)}";
                }
                else
                {
                    _namespace = @namespace;
                    _tenant = @namespace;
                    _name = $"{FIXED_NAME}-{GetFixedNameSuffix(_serverUrls)}-{@namespace}";
                }
            }
            else
            {
                // handle with endpoint, not support at this time.
                throw new ArgumentNullException("ServerAddresses can not be null or empty ");
            }
        }

        public List<string> GetServerUrls()
        {
            return _serverUrls;
        }

        public string GetCurrentServerAddr()
        {
            if (string.IsNullOrWhiteSpace(_currentServerAddr))
            {
                Random random = new Random();
                int index = random.Next(0, _serverUrls.Count);
                _currentServerAddr = _serverUrls[index];
            }

            return _currentServerAddr;
        }

        public void RefreshCurrentServerAddr()
        {
            Random random = new Random();
            int index = random.Next(0, _serverUrls.Count);
            _currentServerAddr = _serverUrls[index];
        }

        public void UpdateCurrentServerAddr(string currentServerAddr)
        {
            _currentServerAddr = currentServerAddr;
        }

        public string GetName() => _name;

        public string GetNamespace() => _namespace;

        public string GetTenant() => _tenant;


        private string GetFixedNameSuffix(List<string> serverIps)
        {
            StringBuilder sb = new StringBuilder(1024);
            string split = "";

            foreach (var item in serverIps)
            {
                sb.Append(split);
                var ip = Regex.Replace(item, "http(s)?://", "");
                sb.Append(ip.Replace(':', '_'));
                split = "-";
            }
            return sb.ToString();
        }

    }
}
